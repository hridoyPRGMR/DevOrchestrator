using System.Threading.Tasks;
using DevOrchestrator.Domain.Models;

namespace DevOrchestrator.Domain.Services;

public interface IAiService
{
    Task<string> AnalyzeCodeAsync(string code, string? language = null);
    Task<AiResponse> GenerateAsync(string prompt, string? model = null);
    Task<AiResponse> EmbedAsync(string text, string? model = null);
}
