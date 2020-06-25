using System.Linq;
using System.Threading.Tasks;
using EsapiEssentials.Plugin;
using VMS.TPS.Common.Model.API;
using System.IO;

namespace DVHAsync
{
    public class EsapiService : EsapiServiceBase<PluginScriptContext>, IEsapiService
    {
        private readonly DoseMetricCalculator _metricCalc;

        public EsapiService(PluginScriptContext context) : base(context)
        {
            _metricCalc = new DoseMetricCalculator();
        }

        public Task<Plan[]> GetPlansAsync() =>
            RunAsync(context => context.Patient.Courses?
                .SelectMany(x => x.PlanSetups)
                .Select(x => new Plan
                {
                    PlanId = x.Id,
                    CourseId = x.Course?.Id,
                })
                .ToArray());

        public Task<string[]> GetStructureIdsAsync(string courseId, string planId) =>
            RunAsync(context =>
            {
                var plan = GetPlan(context.Patient, courseId, planId);
                return plan?.StructureSet?.Structures?.Select(x => x.Id).ToArray() ?? new string[0];
            });

        public Task<PQMSummaryViewModel[]> GetObjectivesAsync(string path) =>
            RunAsync(context =>
            {
                var objectives = Objectives.GetObjectives(path);
                return objectives.ToArray() ?? new PQMSummaryViewModel[0];
            });

        public Task<string> CalculateMetricDoseAsync(string courseId, string planId, string structureId, string metric, PQMSummaryViewModel objective) =>
            RunAsync(context => CalculateMetricDose(context.Patient, courseId, planId, structureId, objective));

        public string CalculateMetricDose(Patient patient, string courseId, string planId, string structureId, PQMSummaryViewModel objective)
        {
            var plan = GetPlan(patient, courseId, planId);
            var planVM = new PlanningItemViewModel(plan);
            var structure = GetStructure(plan, structureId);

            DirectoryInfo constraintDir = new DirectoryInfo(Path.Combine(AssemblyHelper.GetAssemblyDirectory(), "ConstraintTemplates"));
            string firstFileName = constraintDir.GetFiles().FirstOrDefault().ToString();
            string firstConstraintFilePath = Path.Combine(constraintDir.ToString(), firstFileName);

            // make sure the workbook template exists
            if (!System.IO.File.Exists(firstConstraintFilePath))
            {
                System.Windows.MessageBox.Show(string.Format("The template file '{0}' chosen does not exist.", firstConstraintFilePath));
            }
            var structureVM = new StructureViewModel(structure);
            string metric = "";
            var goal = "";
            string result = "";
            string variation = "";
                if (objective.TemplateId == structureId)
                {
                    metric = objective.DVHObjective;
                    goal = objective.Goal;
                    variation = objective.Variation;
                    result = _metricCalc.CalculateMetric(planVM.PlanningItemStructureSet, structureVM, planVM, metric);
                }                 
                else
                    result = "";                
            return result;
        }

        public string EvaluateMetricDose(string result, string goal, string variation)
        {
            var met = "";
            met = _metricCalc.EvaluateMetric(result, goal, variation);
            return met;
        }

        public Task<string> EvaluateMetricDoseAsync(string result, string goal, string variation) =>
            RunAsync(context => EvaluateMetricDose(result, goal, variation));

        private PlanSetup GetPlan(Patient patient, string courseId, string planId) =>
            GetCourse(patient, courseId)?.PlanSetups?.FirstOrDefault(x => x.Id == planId);

        private Course GetCourse(Patient patient, string courseId) =>
            patient?.Courses?.FirstOrDefault(x => x.Id == courseId);

        private Structure GetStructure(PlanSetup plan, string structureId) =>
            plan?.StructureSet?.Structures?.FirstOrDefault(x => x.Id == structureId);

    }
}