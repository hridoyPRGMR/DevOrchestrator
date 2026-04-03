using System.Threading.Tasks;
using DevOrchestrator.Domain.Models;

namespace DevOrchestrator.Domain.Services;

public interface IUsageTracker
{
    /// <summary>
    /// Logs AI usage to the database for cost tracking and analytics.
    /// </summary>
    Task LogUsageAsync(AiResponse response, string requestId, Guid? repositoryId = null, string operationType = "generate");
}