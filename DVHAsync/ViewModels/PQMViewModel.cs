using System.Collections.ObjectModel;
using VMS.TPS.Common.Model.API;

namespace DVHAsyncCompare
{
    public class PQMViewModel
    {
        public string TemplateId { get; set; }
        public string[] TemplateCodes { get; set; }
        public string[] TemplateAliases { get; set; }
        public Structure Structure { get; set; }
        public ObservableCollection<StructureViewModel> StructureList { get; set; }
        public string StructureName { get; set; }
        public string StructVolume { get; set; }
        public string DVHObjective { get; set; }
        public string Metric { get; set; }
        public string Eval { get; set; }
        public string Goal { get; set; }
        public string Achieved { get; set; }
        public string Met { get; set; }
        public string Variation { get; set; }
        public string Priority { get; set; }
        public bool isCalculated { get; set; }
        public double AchievedPercentageOfGoal { get; set; }
        public PlanningItemViewModel ActivePlanningItem { get; set; }
    }
}