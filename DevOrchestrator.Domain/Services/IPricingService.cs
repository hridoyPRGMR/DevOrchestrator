using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IPricingService
{
    /// <summary>
    /// Calculates the cost for AI operations based on model and token usage.
    /// </summary>
    Task<decimal> CalculateCostAsync(string model, int inputTokens, int outputTokens);
}