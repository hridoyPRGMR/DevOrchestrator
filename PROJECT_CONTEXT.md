# DevOrchestrator - Project Context

## Overview
DevOrchestrator is a .NET 10 web API service that orchestrates development tools and provides GitHub repository integration with caching, background sync, and MCP (Model Context Protocol) tools.

## Architecture
**Clean Architecture** with 6 layers:
- **Api** - ASP.NET Core web API controllers and startup
- **Application** - Business logic and use cases
- **Domain** - Entities, interfaces, and domain services
- **Infrastructure** - External concerns (EF Core, Redis, GitHub API)
- **Contracts** - DTOs and data transfer objects
- **Tools** - MCP tool implementations for AI agents

## Technologies
- **Runtime**: .NET 10.0
- **Framework**: ASP.NET Core Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis (IDistributedCache)
- **GitHub Integration**: Octokit.net
- **Resilience**: Polly (retry policies, circuit breakers)
- **Logging**: Serilog (structured JSON logging)
- **Background Jobs**: IHostedService

## Key Components

### Domain Layer
- **Entities**: GitHubRepository, GitHubFile, GitHubCommit
- **Services**: IGitHubCacheService, IGitHubApplicationService, IGitHubSyncService

### Application Layer
- **GitHubApplicationService**: Business logic for file operations with filtering/limits

### Infrastructure Layer
- **GitHubService**: Octokit wrapper with Polly resilience
- **GitHubCacheService**: Redis caching implementation
- **GitHubSyncService**: SHA-based sync with staleness checking
- **RepoSyncWorker**: Background service for automatic repo refresh
- **AppDbContext**: EF Core context with PostgreSQL

### API Layer
- **BookingsController**: Sample controller (may be placeholder)
- **Program.cs**: DI registration and Serilog setup
- **appsettings.json**: Configuration for Redis, BackgroundSync, Serilog

### Tools Layer
- **GetGitHubFilesTool**: MCP tool for listing repository files
- **GetGitHubFileContentTool**: MCP tool for fetching file content
- **Standard Error Format**: `{success: bool, error?: string, details?: string}`

## Database Schema
```sql
GitHubRepositories:
- Id (PK)
- Owner, Name, Description
- DefaultBranch, Language
- LastSyncedAt, LastAccessedAt
- CreatedAt, UpdatedAt

GitHubFiles:
- Id (PK)
- RepositoryId (FK)
- Path, Name, Size, Sha
- IsDirectory, LastModified
- CreatedAt, UpdatedAt

GitHubCommits:
- Id (PK)
- RepositoryId (FK)
- Sha, Message, Author
- CommitDate
- CreatedAt
```

## Configuration Sections
- **ConnectionStrings**: PostgreSQL, Redis
- **GitHub**: Token, ApiUrl
- **BackgroundSync**: IntervalMinutes, Enabled
- **Serilog**: JSON formatting, console sink

## Caching Strategy
**3-Level Hierarchy**:
1. **Redis Cache** (fast, distributed)
2. **Database** (persistent, structured)
3. **GitHub API** (source of truth)

**Cache Keys**:
- `github:repo:{owner}:{repo}:files:{path}:{branch}`
- `github:repo:{owner}:{repo}:file:{path}:{branch}`

## Sync Logic
- **SHA-based Updates**: Only sync when GitHub SHA differs
- **Staleness Check**: Configurable time-based refresh
- **Access Tracking**: LastAccessedAt updates trigger background sync
- **Background Worker**: Periodic sync of recently accessed repos

## Resilience Features
- **Polly Policies**: Exponential backoff for rate limits
- **Circuit Breaker**: Prevents cascade failures
- **Timeout Handling**: Configurable request timeouts
- **Retry Logic**: Automatic retry on transient failures

## Build & Run
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run database migrations
dotnet ef database update --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api

# Run API
dotnet run --project DevOrchestrator.Api
```

## Recent Improvements (Phases 1-6)
1. **Database Constraints**: Indexes and foreign keys
2. **SHA Sync**: Efficient change detection
3. **Application Layer**: Proper separation of concerns
4. **Redis Caching**: Distributed cache layer
5. **Background Sync**: Automatic refresh with rate limiting
6. **Observability**: Serilog structured logging and error handling

## MCP Tools
- **get_github_files**: List repository files with filtering
- **get_github_file_content**: Fetch individual file content
- Both return standardized success/error responses

## Development Notes
- Uses Clean Architecture for maintainability
- Comprehensive error handling and logging
- Production-ready with caching and resilience
- Designed for AI agent integration via MCP protocol</content>
<parameter name="filePath">c:\Users\Shaon\Desktop\DevOrchestrator\PROJECT_CONTEXT.md