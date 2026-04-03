# DevOrchestrator
# Api UI Endpoint => http://localhost:5112/scalar




# Database -> 
# dotnet ef migrations add Intial --project ..\DevOrchestrator.Infrastructure\DevOrchestrator.Infrastructure.csproj --startup-project .
# dotnet ef database update --project ..\DevOrchestrator.Infrastructure\DevOrchestrator.Infrastructure.csproj --startup-project .


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