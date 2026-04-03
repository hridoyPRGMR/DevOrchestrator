using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IContextBuilder
{
    /// <summary>
    /// Builds context for a query by finding relevant code chunks and assembling them into a coherent context.
    /// </summary>
    Task<string> BuildContextAsync(Guid repositoryId, string query, int maxTokens = 8000);
}