using DevOrchestrator.Application.Services;
using DevOrchestrator.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevOrchestrator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<BookingService>();
        services.AddScoped<IGitHubApplicationService, GitHubApplicationService>();

        return services;
    }
}

