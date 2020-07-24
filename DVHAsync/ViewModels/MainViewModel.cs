using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DVHAsyncCompare
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEsapiService _esapiService;
        private readonly IDialogService _dialogService;

        public MainViewModel(IEsapiService esapiService, IDialogService dialogService)
        {
            _esapiService = esapiService;
            _dialogService = dialogService;
        }

        private Plan[] _plans;
        public Plan[] Plans
        {
            get => _plans;
            set => Set(ref _plans, value);
        }

        private Plan _selectedPlan;
        public Plan SelectedPlan
        {
            get => _selectedPlan;
            set => Set(ref _selectedPlan, value);
        }

        private Plan _selectedPlanCompare1;
        public Plan SelectedPlanCompare1
        {
            get => _selectedPlanCompare1;
            set => Set(ref _selectedPlanCompare1, value);
        }

        private Plan _selectedPlanCompare2;
        public Plan SelectedPlanCompare2
        {
            get => _selectedPlanCompare2;
            set => Set(ref _selectedPlanCompare2, value);
        }

        private Plan _selectedPlanCompare3;
        public Plan SelectedPlanCompare3
        {
            get => _selectedPlanCompare3;
            set => Set(ref _selectedPlanCompare3, value);
        }

        private ObservableCollection<MetricResult> _metricResults;
        public ObservableCollection<MetricResult> MetricResults
        {
            get => _metricResults;
            set => Set(ref _metricResults, value);
        }

        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand AnalyzePlanCommand => new RelayCommand(AnalyzePlan);

        private async void Start()
        {
            Plans = await _esapiService.GetPlansAsync();
        }

        private async void AnalyzePlan()
        {
            var courseId = SelectedPlan?.CourseId;
            var planId = SelectedPlan?.PlanId;
            
            if (courseId == null || planId == null)
                return;

            var structureIds = await _esapiService.GetStructureIdsAsync(courseId, planId);

            DirectoryInfo constraintDir = new DirectoryInfo(Path.Combine(AssemblyHelper.GetAssemblyDirectory(), "ConstraintTemplates"));
            string firstFileName = constraintDir.GetFiles().FirstOrDefault().ToString();
            string firstConstraintFilePath = Path.Combine(constraintDir.ToString(), firstFileName);

            // make sure the workbook template exists
            if (!System.IO.File.Exists(firstConstraintFilePath))
            {
                System.Windows.MessageBox.Show(string.Format("The template file '{0}' chosen does not exist.", firstConstraintFilePath));
            }
            var pqms = Objectives.GetObjectives(firstConstraintFilePath);

            _dialogService.ShowProgressDialog("Calculating dose metrics", structureIds.Length,
                async progress =>
                {
                    MetricResults = new ObservableCollection<MetricResult>();
                    foreach (var structureId in structureIds)
                    {

                        string result = "";
                        string resultCompare1 = "";
                        string resultCompare2 = "";
                        string resultCompare3 = "";
                        string metric = "";
                        string goal = "";
                        string met = "";
                        string variation = "";
                        try
                        {
                            foreach (var pqm in pqms)
                            {
                                if (pqm.TemplateId == structureId)
                                {
                                    result = "";
                                    resultCompare1 = "";
                                    resultCompare2 = "";
                                    resultCompare3 = "";
                                    metric = pqm.DVHObjective;
                                    goal = pqm.Goal;
                                    variation = pqm.Variation;
                                    result = await _esapiService.CalculateMetricDoseAsync(courseId, planId, structureId, pqm.TemplateId, pqm.DVHObjective, pqm.Goal, pqm.Variation);
                                    met = await _esapiService.EvaluateMetricDoseAsync(result, goal, variation);

                                    var planCompare1 = SelectedPlanCompare1?.PlanId;
                                    if (planCompare1 != null)
                                        resultCompare1 = await _esapiService.CalculateMetricDoseAsync(courseId, planCompare1, structureId, pqm.TemplateId, pqm.DVHObjective, pqm.Goal, pqm.Variation);

                                    var planCompare2 = SelectedPlanCompare2?.PlanId;
                                    if (planCompare2 != null)
                                        resultCompare2 = await _esapiService.CalculateMetricDoseAsync(courseId, planCompare2, structureId, pqm.TemplateId, pqm.DVHObjective, pqm.Goal, pqm.Variation);

                                    var planCompare3 = SelectedPlanCompare3?.PlanId;
                                    if (planCompare3 != null)
                                        resultCompare3 = await _esapiService.CalculateMetricDoseAsync(courseId, planCompare3, structureId, pqm.TemplateId, pqm.DVHObjective, pqm.Goal, pqm.Variation);

                                    MetricResults.Add(new MetricResult
                                    {
                                        TemplateId = structureId,
                                        Metric = metric,
                                        Goal = goal,
                                        Met = met,
                                        Result = result,
                                        ResultCompare1 = resultCompare1,
                                        ResultCompare2 = resultCompare2,
                                        ResultCompare3 = resultCompare3
                                    });
                                }
                            }                         
                        }
                        catch
                        {
                            result = "";
                        }
                        progress.Increment();
                    }
                });
        }
    }

}