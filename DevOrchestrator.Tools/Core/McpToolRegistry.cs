using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DevOrchestrator.Tools.Core;

public interface IMcpToolRegistry
{
    IMcpTool? Get(string name);
    IEnumerable<string> GetToolNames();
}

public class McpToolRegistry : IMcpToolRegistry
{
    private readonly IDictionary<string, Func<IMcpTool>> _toolFactories;

    public McpToolRegistry(IServiceProvider serviceProvider, IEnumerable<IMcpTool> tools)
    {
        _toolFactories = new Dictionary<string, Func<IMcpTool>>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var tool in tools)
        {
            if (!string.IsNullOrWhiteSpace(tool.Name))
            {
                var toolType = tool.GetType();
                _toolFactories[tool.Name] = () => (IMcpTool)ActivatorUtilities.CreateInstance(serviceProvider, toolType);
            }
        }
    }

    public IMcpTool? Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (_toolFactories.TryGetValue(name, out var factory))
            return factory();

        return null;
    }

    public IEnumerable<string> GetToolNames() => _toolFactories.Keys;
}
