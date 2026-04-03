using DevOrchestrator.Tools.Core;
using DevOrchestrator.Tools.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace DevOrchestrator.Tools;

public static class DependencyInjection
{
    public static IServiceCollection AddToolServices(this IServiceCollection services)
    {
        services.AddTransient<IMcpTool, GetTimeTool>();
        services.AddTransient<IMcpTool, SearchCodeTool>();
        services.AddTransient<IMcpTool, GetFileTool>();
        services.AddTransient<IMcpTool, AnalyzeCodeTool>();
        services.AddTransient<IMcpTool, GetGitHubFilesTool>();
        services.AddTransient<IMcpTool, GetGitHubFileContentTool>();
        
        services.AddSingleton<IMcpToolRegistry>(provider =>
        {
            var tools = provider.GetRequiredService<IEnumerable<IMcpTool>>();
            return new McpToolRegistry(provider, tools);
        });

        return services;
    }
}
