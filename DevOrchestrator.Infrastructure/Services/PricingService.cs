using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace DevOrchestrator.Infrastructure.Services;

public class PricingService : IPricingService
{
    private readonly Dictionary<string, (decimal InputCostPerToken, decimal OutputCostPerToken)> _pricingRates;

    public PricingService(IConfiguration configuration)
    {
        _pricingRates = new Dictionary<string, (decimal, decimal)>
        {
            // GPT-4 pricing (as of 2024)
            ["gpt-4"] = (0.00003m, 0.00006m), // $0.03 per 1K input tokens, $0.06 per 1K output tokens
            ["gpt-4-32k"] = (0.00006m, 0.00012m),
            ["gpt-4-turbo"] = (0.00001m, 0.00003m),
            ["gpt-4-turbo-preview"] = (0.00001m, 0.00003m),
            ["gpt-4-0125-preview"] = (0.00001m, 0.00003m),
            ["gpt-4-1106-preview"] = (0.00001m, 0.00003m),

            // GPT-3.5 pricing
            ["gpt-3.5-turbo"] = (0.0000015m, 0.000002m), // $0.0015 per 1K input, $0.002 per 1K output
            ["gpt-3.5-turbo-16k"] = (0.000003m, 0.000004m),
            ["gpt-3.5-turbo-0125"] = (0.0000005m, 0.0000015m),
            ["gpt-3.5-turbo-1106"] = (0.000001m, 0.000002m),

            // Embedding models
            ["text-embedding-ada-002"] = (0.0000001m, 0m), // $0.0001 per 1K tokens (input only)
            ["text-embedding-3-small"] = (0.00000002m, 0m), // $0.00002 per 1K tokens
            ["text-embedding-3-large"] = (0.00000013m, 0m), // $0.00013 per 1K tokens

            // Default fallback
            ["default"] = (0.00001m, 0.00002m)
        };

        // Load custom pricing from configuration if available
        var customPricing = configuration.GetSection("AiPricing");
        if (customPricing.Exists())
        {
            foreach (var item in customPricing.GetChildren())
            {
                if (item.Key != null && decimal.TryParse(item["InputCostPerToken"], out var inputCost) &&
                    decimal.TryParse(item["OutputCostPerToken"], out var outputCost))
                {
                    _pricingRates[item.Key] = (inputCost, outputCost);
                }
            }
        }
    }

    public Task<decimal> CalculateCostAsync(string model, int inputTokens, int outputTokens)
    {
        if (string.IsNullOrEmpty(model))
            model = "default";

        if (!_pricingRates.TryGetValue(model, out var rates))
        {
            // Try to find a matching model prefix (e.g., "gpt-4" for "gpt-4-0613")
            var matchingKey = _pricingRates.Keys.FirstOrDefault(k =>
                model.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            rates = matchingKey != null ? _pricingRates[matchingKey] : _pricingRates["default"];
        }

        // Convert tokens to thousands and calculate cost
        decimal inputCost = (inputTokens / 1000.0m) * rates.InputCostPerToken;
        decimal outputCost = (outputTokens / 1000.0m) * rates.OutputCostPerToken;

        return Task.FromResult(inputCost + outputCost);
    }
}