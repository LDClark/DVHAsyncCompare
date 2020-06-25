using System.Threading.Tasks;

namespace DVHAsync
{
    public interface IEsapiService
    {
        Task<Plan[]> GetPlansAsync();
        Task<string[]> GetStructureIdsAsync(string courseId, string planId);
        Task<string> CalculateMetricDoseAsync(string courseId, string planId, string structureId, string metric, PQMSummaryViewModel objective);
        Task<string> EvaluateMetricDoseAsync(string result, string goal, string variation);
    }
}