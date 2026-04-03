using System.Threading.Tasks;

namespace DevOrchestrator.Domain.Services;

public interface ICacheService
{
    Task<string?> GetAsync(string hashKey);
    Task SetAsync(string hashKey, string prompt, string response);
}
