using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly ICacheService _cacheService;

    public AiService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<string> AnalyzeCodeAsync(string code, string? language = null)
    {
        string hashKey = GenerateHashKey(code);
        
        // Check cache first
        var cached = await _cacheService.GetAsync(hashKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return $"[CACHED] {cached}";
        }

        // Simulate AI analysis
        await Task.Delay(100);

        var lineCount = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var issues = new System.Collections.Generic.List<string>();

        if (code.Contains("TODO", StringComparison.OrdinalIgnoreCase))
            issues.Add("Contains TODO marker");

        if (code.Contains("var ", StringComparison.OrdinalIgnoreCase))
            issues.Add("Contains implicit 'var' usage; consider explicit typing for clarity");

        var result = $"Code Analysis: {lineCount} lines, {issues.Count} issues found. " +
                     (issues.Count > 0 ? string.Join("; ", issues) : "No issues detected.");

        // Cache the result
        await _cacheService.SetAsync(hashKey, code, result);

        return result;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        string hashKey = GenerateHashKey(prompt);
        
        // Check cache first
        var cached = await _cacheService.GetAsync(hashKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return $"[CACHED] {cached}";
        }

        // Simulate AI generation
        await Task.Delay(100);
        
        var result = $"Generated response for: {prompt}";

        // Cache the result
        await _cacheService.SetAsync(hashKey, prompt, result);

        return result;
    }

    private static string GenerateHashKey(string input)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash);
    }
}

