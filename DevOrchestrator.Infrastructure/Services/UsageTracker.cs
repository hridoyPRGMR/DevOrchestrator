using System;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Models;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Infrastructure.Persistence;

namespace DevOrchestrator.Infrastructure.Services;

public class UsageTracker : IUsageTracker
{
    private readonly AppDbContext _context;

    public UsageTracker(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogUsageAsync(AiResponse response, string requestId, Guid? repositoryId = null, string operationType = "generate")
    {
        var usageLog = new AiUsageLog
        {
            RequestId = requestId,
            RepositoryId = repositoryId,
            OperationType = operationType,
            Model = response.Model,
            InputTokens = response.InputTokens,
            OutputTokens = response.OutputTokens,
            Cost = response.Cost,
            CacheHit = response.CacheHit,
            CreatedAt = DateTime.UtcNow
        };

        _context.AiUsageLogs.Add(usageLog);
        await _context.SaveChangesAsync();
    }
}