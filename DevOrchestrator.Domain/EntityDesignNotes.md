# Domain Entity Design Notes

## Purpose
This file captures the entity model decisions, file layout, and conventions used for the five .NET+AI domain contexts added to `DevOrchestrator.Domain`.

## Project contexts and namespaces
- `DevOrchestrator.Domain.Coworking`
- `DevOrchestrator.Domain.Travel`
- `DevOrchestrator.Domain.Support`
- `DevOrchestrator.Domain.Resume`
- `DevOrchestrator.Domain.DocumentSummarizer`

## Folder layout
- `DevOrchestrator.Domain/Entities/Coworking/`
- `DevOrchestrator.Domain/Entities/Travel/`
- `DevOrchestrator.Domain/Entities/Support/`
- `DevOrchestrator.Domain/Entities/Resume/`
- `DevOrchestrator.Domain/Entities/DocumentSummarizer/`

Each entity has its own file, e.g.:
- `User.cs`
- `Booking.cs`
- `Payment.cs`
- `ChatMessage.cs`
- `AIQueryLog.cs`

## Domain modeling decisions
- Used `Guid` for entity identifiers
- Used `DateTimeOffset` for timestamps and date/time fields
- Used `required` properties for mandatory values
- Used `string` fields for JSON-like or text values (`Preferences`, `Activities`, etc.)
- Kept the domain layer persistence-agnostic; no EF-specific attributes were added
- Navigation properties were included to model relationships
- Collections are initialized with `new List<T>()`

## Context-specific entities
### Coworking Space Management + AI Assistant
- `User`
- `Workspace`
- `Booking`
- `Payment`
- `ChatMessage`
- `AIQueryLog`

### Travel Itinerary Planner with AI Chat
- `User`
- `Trip`
- `DayPlan`
- `Activity`
- `ChatMessage`
- `Place`

### AI Customer Support Ticket Analyzer
- `User`
- `Ticket`
- `TicketSummary`
- `KnowledgeBaseArticle`
- `ChatMessage`

### AI Resume & Cover Letter Advisor
- `User`
- `Resume`
- `JobPosting`
- `CoverLetter`
- `ChatMessage`

### AI-Powered Document Summarizer & Q&A
- `User`
- `Document`
- `DocumentSummary`
- `DocumentChunk`
- `ChatMessage`

## Validation
- Verified with `dotnet build DevOrchestrator.slnx -c Debug`
- Build succeeded without errors for all projects

## Notes for future work
- If persistence is added later, keep mapping logic in `DevOrchestrator.Infrastructure`
- Consider consolidating shared `User` concepts if the same user model is desired across contexts
- If a separate shared domain layer is preferred, move common entities into a shared namespace
