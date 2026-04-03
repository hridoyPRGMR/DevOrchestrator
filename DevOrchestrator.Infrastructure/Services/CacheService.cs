using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Entities;
using DevOrchestrator.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DevOrchestrator.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly Persistence.AppDbContext _dbContext;

    public CacheService(Persistence.AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetAsync(string hashKey)
    {
        if (string.IsNullOrWhiteSpace(hashKey))
            return null;

        var cached = await _dbContext.AiCaches
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.HashKey == hashKey);

        return cached?.Response;
    }

    public async Task SetAsync(string hashKey, string prompt, string response)
    {
        if (string.IsNullOrWhiteSpace(hashKey))
            return;

        var existing = await _dbContext.AiCaches
            .FirstOrDefaultAsync(c => c.HashKey == hashKey);

        if (existing != null)
        {
            existing.Response = response;
            existing.CreatedAt = DateTime.UtcNow;
            _dbContext.AiCaches.Update(existing);
        }
        else
        {
            var newCache = new AiCache
            {
                Id = Guid.NewGuid(),
                HashKey = hashKey,
                Prompt = prompt,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.AiCaches.AddAsync(newCache);
        }

        await _dbContext.SaveChangesAsync();
    }
}
