# DevOrchestrator

A comprehensive AI-powered development assistant built with Clean Architecture, providing intelligent code analysis, semantic search, multi-tool agent orchestration, and cost tracking.

## 🚀 Features

### 🤖 AI Agent System
- **Natural Language Queries**: Ask questions like "Find bugs in my authentication code"
- **Multi-Tool Orchestration**: Automatically combines analysis, search, and embedding tools
- **Contextual Responses**: Repository-aware answers with file-specific insights
- **Conversation Continuity**: Multi-turn interactions with session management

### 🔍 Semantic Code Search
- **Embedding-Based Search**: Find similar code using AI embeddings
- **Intelligent Chunking**: Language-aware code splitting for optimal search
- **Repository-Wide Search**: Search across entire codebases semantically

### 📊 Cost & Usage Tracking
- **Token Accounting**: Precise tracking of AI API usage
- **Cost Calculation**: Real-time cost monitoring across different models
- **Usage Analytics**: Comprehensive logging for optimization

### 🛠️ MCP Tool Ecosystem
- **Modular Tools**: Pluggable tools for different development tasks
- **Dynamic Execution**: Agent automatically selects appropriate tools
- **Extensible Framework**: Easy to add new capabilities

## 🏗️ Architecture

Built with Clean Architecture in .NET 10.0:

```
DevOrchestrator/
├── Api/                 # REST API endpoints
├── Application/         # Application services
├── Domain/             # Core business logic & entities
├── Infrastructure/     # Data access & external APIs
├── Tools/              # MCP tools for orchestration
└── Shared/             # Common utilities
```

## 🗄️ Technology Stack

- **Backend**: .NET 10.0, C#
- **Database**: PostgreSQL with pgvector
- **Cache**: Redis
- **AI**: OpenAI API + Ollama (local embeddings)
- **Architecture**: Clean Architecture, Dependency Injection
- **Logging**: Serilog
- **Resilience**: Polly policies

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL with pgvector extension
- Redis (optional, for caching)
- OpenAI API key (optional, falls back to deterministic responses)

### Setup
1. **Clone and build**:
   ```bash
   git clone <repository-url>
   cd DevOrchestrator
   dotnet build
   ```

2. **Database setup**:
   ```bash
   # Create migration
   dotnet ef migrations add Initial --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api

   # Update database
   dotnet ef database update --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api
   ```

3. **Configure settings**:
   - Copy `appsettings.json` and configure your API keys and connection strings
   - Set OpenAI API key for AI features
   - Configure PostgreSQL and Redis connections

4. **Run the application**:
   ```bash
   dotnet run --project DevOrchestrator.Api
   ```

## 📡 API Usage

### Agent Queries
```bash
# Natural language code analysis
curl -X POST http://localhost:5000/api/agent/query \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Analyze this authentication code for security issues",
    "context": {
      "filePaths": ["Controllers/AuthController.cs"]
    }
  }'
```

### Direct Tool Execution
```bash
# Use MCP tools directly
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "tool": "analyze_code",
    "arguments": {
      "code": "public class User { public string Password { get; set; } }"
    }
  }'
```

## 🛠️ Available Tools

- **analyze_code**: AI-powered code analysis and suggestions
- **search_code**: Text-based code search
- **get_file**: Retrieve file contents
- **find_similar_code**: Semantic code search using embeddings
- **get_repository_info**: Repository metadata and status
- **get_time**: Current time information

## 📊 Monitoring

- **API Documentation**: Visit `http://localhost:5000/scalar` for OpenAPI docs
- **Health Checks**: System health monitoring endpoints
- **Logging**: Structured logs with Serilog
- **Metrics**: Performance and usage tracking

## 🔧 Development

### Adding New Features
1. **New Service**: Add interface to Domain, implementation to Infrastructure
2. **New Tool**: Implement `IMcpTool` in Tools/Implementations
3. **New API**: Add controller to Api/Controllers
4. **Database Changes**: Create EF migration and update schema

### Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📈 Roadmap

- [x] **Phase 1**: Data models and entities
- [x] **Phase 2**: Embedding services
- [x] **Phase 3**: Context building and chunking
- [x] **Phase 4**: Token tracking and cost calculation
- [x] **Phase 5**: Multi-tool agent orchestration
- [x] **Phase 6**: API integration and user interface
- [ ] **Phase 7**: Advanced agent capabilities (conversation memory, multi-agent collaboration)
- [ ] **Phase 8**: IDE integrations (VS Code, Cursor)
- [ ] **Phase 9**: Team collaboration features
- [ ] **Phase 10**: Enterprise deployment and scaling

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Api UI Endpoint**: http://localhost:5112/scalar

**Database Commands**:
```bash
# Create migration
dotnet ef migrations add MigrationName --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api

# Update database
dotnet ef database update --project DevOrchestrator.Infrastructure --startup-project DevOrchestrator.Api
```


### 🔍 **Problem Statement**
With the increasing use of AI tools in development:
* Developers repeatedly perform similar tasks (code analysis, debugging, documentation)
* AI usage is often **stateless**, leading to repeated context submission and higher costs
* There is **no centralized intelligence layer** to automate development workflows
* AI outputs are not reusable across the team
---

### 💡 **Proposed Solution: Code Agent Platform**
Develop a **Code Agent System** that can:
* Act as an intelligent assistant for developers
* Execute tasks like analyzing code, generating features, reviewing pull requests
* Maintain shared context across the team
* Integrate with tools like Cursor and GitHub

This system will go beyond a chatbot and function as an **automated development agent**.
---

### ⚙️ **Core Capabilities**
1. **Autonomous Code Agents**

   * Analyze codebases
   * Suggest improvements
   * Generate code and documentation
   * Assist in debugging

2. **MCP Tool Framework**
   * Modular tools (e.g., `analyze_code`, `search_repo`, `generate_feature`)
   * Dynamic execution based on developer requests

3. **Shared Context Engine**
   * Store project knowledge, summaries, and AI outputs
   * Enable reuse across developers

4. **GitHub Integration**
   * Read repositories
   * Analyze pull requests
   * Provide automated review suggestions

5. **AI Cost Optimization**
   * Context filtering (send only relevant data)
   * Response caching
   * Reuse of previous AI outputs

---

### 🏗️ **High-Level Architecture**
* **API Layer (.NET)** → Handles requests from developers/tools
* **Application Layer** → Orchestrates agent workflows
* **Agent Engine** → Decides which tools to execute
* **MCP Tool Layer** → Executes tasks (code analysis, GitHub operations)
* **Context Engine** → Retrieves and optimizes relevant data
* **Infrastructure Layer**:

  * PostgreSQL → metadata
  * Vector database → embeddings
  * Redis → caching
  * AI APIs → intelligence layer
--

### 🚀 **Implementation Plan (MVP)**

**Phase 1: Foundation** - Completed
* Project setup (.NET 10.0 + PostgreSQL with Entity Framework Core)
* MCP endpoint + tool framework with IMcpTool interface and McpToolRegistry
* Basic agent execution flow via McpController

**Phase 2: Core Agent Features** - Completed
* Implemented 6 MCP tools: get_time, get_file, search_code, analyze_code, get_github_files, get_github_file_content
* Code analysis tool with AI integration
* Repository search and file retrieval tools
* GitHub integration with read-only access, PAT authentication, and database synchronization
* Database persistence for GitHub repositories and files

**Phase 3: Intelligence Layer** - Completed
* AI service with SHA256-based response caching (AiCache entity)
* Context optimization to minimize AI API calls and costs
* Caching layer to reuse previous AI outputs

**Phase 4: Production Hardening** - Completed
* Database constraints and indexes for GitHub entities
* SHA-based update strategy to avoid unnecessary syncs
* Staleness checks with configurable intervals
* Application layer refactoring: moved business logic from tools to GitHubApplicationService
* Proper layering: Tools → ApplicationService → SyncService → GitHubService
* Redis caching layer for fast in-memory cache (TTL: 1 hour for files, 24 hours for repos)

**Phase 5: Advanced Caching & Background Sync** - Completed
* Background worker for periodic repository sync (every 5 minutes for active repos)
* Rate limit handling with Polly retry policies (exponential backoff for 403 errors)
* Repository access tracking with LastAccessedAt field
* Automatic background refresh for repositories accessed in last 24 hours

**Phase 6: Observability & Error Handling** - Planned
* Serilog structured logging
* Standard error responses for tools
* Comprehensive exception handling

**Phase 6: Observability & Error Handling** - Planned
* Serilog structured logging
* Standard error responses for tools
* Comprehensive exception handling

---

### ⚠️ **Considerations**
* Ensuring accuracy of AI-generated outputs
* Managing context relevance and versioning
* Monitoring infrastructure and AI usage costs
---