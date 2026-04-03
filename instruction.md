# 🧠 AI CODE AGENT INSTRUCTION CONTEXT

## Project: MCP-Based Code Agent Platform (.NET 10)

---

## 🎯 PURPOSE

You (AI Code Agent) are building a **backend system** that acts as an intelligent developer assistant.

This system:

* Receives requests (via MCP protocol)
* Selects and executes tools
* Uses shared context (code, summaries, history)
* Calls AI only when necessary
* Optimizes cost by reducing repeated AI usage

---

## ❗ DO NOT ASSUME

* Do NOT invent features not defined here
* Do NOT build a chatbot UI
* Do NOT add authentication (for now)
* Do NOT tightly couple components
* Do NOT skip layers (must follow architecture)

If something is unclear → keep it minimal and extendable.

---

## 🏗️ ARCHITECTURE RULES

Follow **Clean Architecture** strictly:

```text
Api → Application → Domain → Infrastructure
```

### Layer Responsibilities:

* **Api**

  * Expose endpoints
  * No business logic

* **Application**

  * Orchestrates workflows
  * Calls tools and services

* **Domain**

  * Core models and interfaces
  * No external dependencies

* **Infrastructure**

  * Database
  * GitHub API
  * AI API
  * External services

---

## 🔌 CORE CONCEPT: MCP SYSTEM

### Entry Point

```http
POST /mcp
```

### Request Format

```json
{
  "tool": "tool_name",
  "arguments": {}
}
```

---

## 🧠 EXECUTION FLOW (MANDATORY)

```text
1. Receive request
2. Identify tool
3. Fetch tool from registry
4. Execute tool
5. Return result
```

---

## 🧰 TOOL SYSTEM (CRITICAL)

### Interface

```csharp
public interface IMcpTool
{
    string Name { get; }
    Task<object> ExecuteAsync(Dictionary<string, object> args);
}
```

---

### Rules

* Each tool = single responsibility
* Tools must be independent
* No direct DB or API calls inside tools → use services
* Tools must be async

---

### Initial Tools (must implement first)

```text
- get_time
- get_file
- search_code
- analyze_code
```

---

## 🧠 AGENT ENGINE

Responsibilities:

* Receive MCP request
* Select tool
* Execute tool
* Handle response

### Rules:

* No business logic duplication
* Use ToolRegistry
* Keep logic minimal

---

## 🧩 TOOL REGISTRY

* Stores all tools
* Resolves tool by name

```csharp
IMcpTool Get(string name);
```

---

## 🗄️ DATA STORAGE (PostgreSQL)

### Required Tables

#### Projects

```text
Id
Name
RepositoryUrl
```

#### Files

```text
Id
ProjectId
Path
Content
EmbeddingVector (optional initially)
```

#### AiCache

```text
Id
HashKey
Prompt
Response
```

---

## ⚡ CACHING RULE (IMPORTANT)

Before calling AI:

```text
IF cached response exists → return it
ELSE → call AI → store result
```

---

## 🤖 AI USAGE RULES

* Only call AI when necessary
* Always minimize prompt size
* Prefer reused context over raw data
* Never send full codebase

---

## 🔍 CONTEXT ENGINE (SIMPLIFIED FOR MVP)

Responsibilities:

* Find relevant files
* Return minimal data

### For now:

* Use simple DB query (no vector DB required yet)

---

## 🔗 GITHUB INTEGRATION (BASIC)

Capabilities:

* Fetch repository files
* Store in database

### Rules:

* Read-only
* No write operations yet

---

## 🚀 BUILD ORDER (STRICT)

### STEP 1

* Setup solution structure
* Setup PostgreSQL
* Create DbContext

---

### STEP 2

* Create IMcpTool
* Create ToolRegistry
* Create /mcp endpoint

---

### STEP 3

* Implement get_time tool
* Test execution flow

---

### STEP 4

* Implement get_file + search_code
* Connect DB

---

### STEP 5

* Add AI service
* Implement analyze_code tool

---

### STEP 6

* Add caching layer

---

## 📌 CODING RULES

* Use async/await everywhere
* Use dependency injection
* Keep classes small
* Avoid static logic
* Log important actions
* Follow naming conventions

---

## ⚠️ CONSTRAINTS

* No frontend
* No authentication
* No over-engineering
* Keep MVP simple but extensible

---

## ✅ SUCCESS CONDITIONS

* MCP endpoint works
* Tools execute dynamically
* GitHub data stored
* AI tool works
* Cache prevents repeated AI calls

---

## 🧠 FINAL UNDERSTANDING

You are NOT building:

* a CRUD app
* a chatbot

You ARE building:

> A modular AI-powered backend system that executes developer tools using shared context and optimized AI usage

---

## 🔚 END OF INSTRUCTION
