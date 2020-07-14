using System.Threading.Tasks;

namespace DVHAsyncCompare
{
    public interface IEsapiService
    {
        Task<Plan[]> GetPlansAsync();
        Task<string[]> GetStructureIdsAsync(string courseId, string planId);
        Task<string> CalculateMetricDoseAsync(string courseId, string planId, string structureId, string templateId, string dvhObjective, string goal, string variation);
        Task<string> EvaluateMetricDoseAsync(string result, string goal, string variation);
    }
}