# DevOrchestrator Development Instructions

## System Overview
DevOrchestrator is a comprehensive AI-powered development assistant built with Clean Architecture. The system provides intelligent code analysis, semantic search, multi-tool agent orchestration, and cost tracking.

## Architecture Principles
- **Clean Architecture**: Strict separation between layers (Api, Domain, Infrastructure, Tools)
- **Dependency Injection**: All services registered in DI containers
- **Async/Await**: All I/O operations are asynchronous
- **Repository Pattern**: Data access through repository interfaces
- **CQRS Pattern**: Commands and queries separated where appropriate

## Quick Reference for Common Tasks

### Adding a New Service
1. **Domain Interface**: Create `IDomainService.cs` in `DevOrchestrator.Domain/Services/`
2. **Infrastructure Implementation**: Create `DomainService.cs` in `DevOrchestrator.Infrastructure/Services/`
3. **DI Registration**: Add to `DevOrchestrator.Infrastructure/DependencyInjection.cs`
4. **Configuration**: Add settings to `appsettings.json` if needed

### Adding a New MCP Tool
1. **Create Tool Class**: Implement `IMcpTool` in `DevOrchestrator.Tools/Implementations/`
2. **Register Tool**: Add to `DevOrchestrator.Tools/DependencyInjection.cs`
3. **Tool is automatically available** to the agent orchestration system

### Adding a New API Endpoint
1. **Controller**: Create in `DevOrchestrator.Api/Controllers/`
2. **Models**: Define request/response models in `DevOrchestrator.Api/Models/`
3. **Routing**: Use attribute routing with appropriate HTTP methods

### Database Changes
1. **Update Entities**: Modify entities in `DevOrchestrator.Domain/Entities/`
2. **Create Migration**: `dotnet ef migrations add MigrationName --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api`
3. **Apply Migration**: `dotnet ef database update --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api`

## Key Components Reference

### Core Services
- **IAiService**: AI text generation and embedding
- **IAgentEngine**: Multi-tool orchestration
- **IEmbeddingService**: Embedding management
- **IRepositoryFileChunkService**: Code chunk operations
- **IUsageTracker**: Cost and usage logging

### Important Entities
- **GitHubRepository**: Repository metadata
- **RepositoryFile**: File content and embeddings
- **RepositoryFileChunk**: Semantic code chunks
- **AiUsageLog**: Token usage tracking
- **AiResponse**: AI operation results with metadata

### Configuration Sections
- **OpenAI**: API key, base URL, model settings
- **Ollama**: Local embedding service configuration
- **AiPricing**: Cost rates for different models
- **ConnectionStrings**: Database connections
- **Redis**: Caching configuration

## Development Workflow
1. **Feature Branch**: Create from main branch
2. **Implement**: Follow Clean Architecture principles
3. **Test**: Add unit and integration tests
4. **Build**: Ensure clean build with no warnings
5. **Document**: Update memory files and README
6. **PR**: Submit pull request with description

## Common Patterns

### Service Implementation
```csharp
public class MyService : IMyService
{
    private readonly IDependency _dependency;
    private readonly ILogger<MyService> _logger;

    public MyService(IDependency dependency, ILogger<MyService> logger)
    {
        _dependency = dependency;
        _logger = logger;
    }

    public async Task<Result> DoSomethingAsync(Request request)
    {
        // Implementation
    }
}
```

### MCP Tool Implementation
```csharp
public class MyTool : Core.IMcpTool
{
    public string Name => "my_tool";

    public async Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        // Tool logic
        return new { result = "success" };
    }
}
```

### API Controller
```csharp
[ApiController]
[Route("api/myfeature")]
public class MyController : ControllerBase
{
    private readonly IMyService _service;

    public MyController(IMyService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MyRequest request)
    {
        var result = await _service.ProcessAsync(request);
        return Ok(result);
    }
}
```

## Testing Guidelines
- **Unit Tests**: Test services with mocked dependencies
- **Integration Tests**: Test with real database and external APIs
- **API Tests**: Test endpoints with HttpClient
- **Coverage**: Aim for >80% code coverage

## Performance Considerations
- Use async/await for all I/O operations
- Implement caching for expensive operations
- Use batch processing for bulk operations
- Monitor memory usage for large datasets
- Consider pagination for large result sets

## Error Handling
- Use custom exceptions for domain errors
- Log errors with appropriate severity levels
- Return meaningful error messages to clients
- Implement circuit breakers for external API calls
- Use Polly policies for resilience

## Security Best Practices
- Validate all input parameters
- Use parameterized queries to prevent SQL injection
- Implement authentication and authorization
- Store secrets securely (not in code)
- Validate API keys and tokens
- Implement rate limiting for public endpoints

## Deployment Checklist
- [ ] All tests pass
- [ ] Clean build with no warnings
- [ ] Database migrations applied
- [ ] Configuration validated
- [ ] Secrets properly configured
- [ ] Health checks implemented
- [ ] Logging configured
- [ ] Monitoring set up