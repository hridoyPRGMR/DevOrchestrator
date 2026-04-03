using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface IAiService
{
    Task<string> AnalyzeCodeAsync(string code, string? language = null);
    Task<string> GenerateAsync(string prompt);
}
