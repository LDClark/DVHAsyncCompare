using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DVHAsync
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

        private ObservableCollection<MetricResult> _metricResults;
        public ObservableCollection<MetricResult> MetricResults
        {
            get => _metricResults;
            set => Set(ref _metricResults, value);
        }

        public ICommand StartCommand => new RelayCommand(Start);

        private async void Start()
        {
            Plans = await _esapiService.GetPlansAsync();
        }

        public ICommand AnalyzePlanCommand => new RelayCommand(AnalyzePlan);

        private async void AnalyzePlan()
        {
            var courseId = SelectedPlan?.CourseId;
            var planId = SelectedPlan?.PlanId;
            
            //var ss = SelectedPlan?.StructureSet;
            //var planningItem = SelectedPlan?.PlanSetup;
            //PlanningItemViewModel planningItemVM = new PlanningItemViewModel(planningItem);
            

            if (courseId == null || planId == null)
                return;

            var structureIds = await _esapiService.GetStructureIdsAsync(courseId, planId);
            //var planSetups = await _esapiService.GetPlanSetupsAsync();

            DirectoryInfo constraintDir = new DirectoryInfo(Path.Combine(AssemblyHelper.GetAssemblyDirectory(), "ConstraintTemplates"));
            string firstFileName = constraintDir.GetFiles().FirstOrDefault().ToString();
            string firstConstraintFilePath = Path.Combine(constraintDir.ToString(), firstFileName);

            // make sure the workbook template exists
            if (!System.IO.File.Exists(firstConstraintFilePath))
            {
                System.Windows.MessageBox.Show(string.Format("The template file '{0}' chosen does not exist.", firstConstraintFilePath));
                //return;
            }
            //ConstraintViewModel constraints = new ConstraintViewModel(constraintPath);

            //PQMSummaryViewModel[] objectives = Objectives.GetObjectives(constraintPath);
            var pqms = Objectives.GetObjectives(firstConstraintFilePath);

            _dialogService.ShowProgressDialog("Calculating dose metrics", structureIds.Length,
                async progress =>
                {
                    MetricResults = new ObservableCollection<MetricResult>();
                    foreach (var structureId in structureIds)
                    {                       

                        string result = "";
                        string metric = "";
                        string goal = "";
                        string met = "";
                        string variation = "";
                        try
                        {
                            
                            //var pqm = new PQMSummaryViewModel();
                            foreach (var pqm in pqms)
                            {
                                if (pqm.TemplateId == structureId)
                                {
                                    metric = pqm.DVHObjective;
                                    goal = pqm.Goal;
                                    variation = pqm.Variation;
                                    result = await _esapiService.CalculateMetricDoseAsync(courseId, planId, structureId, metric, pqm);
                                    met = await _esapiService.EvaluateMetricDoseAsync(result, goal, variation);
                                    MetricResults.Add(new MetricResult
                                    {
                                        StructureId = structureId,
                                        Metric = metric,
                                        Goal = goal,
                                        Met = met,
                                        Result = result
                                    });
                                }
                            }                         
                        }
                        catch
                        {
                            //result = double.NaN;
                            result = "";
                        }


                        progress.Increment();
                    }
                });
        }
    }
}