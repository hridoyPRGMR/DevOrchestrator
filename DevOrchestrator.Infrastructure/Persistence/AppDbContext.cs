using Coworking = DevOrchestrator.Domain.Coworking;
using Travel = DevOrchestrator.Domain.Travel;
using Support = DevOrchestrator.Domain.Support;
using ResumeDomain = DevOrchestrator.Domain.Resume;
using DocumentSummarizer = DevOrchestrator.Domain.DocumentSummarizer;
using DevOrchestrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevOrchestrator.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AiCache> AiCaches { get; set; } = null!;
    public DbSet<GitHubRepository> GitHubRepositories { get; set; } = null!;
    public DbSet<RepositoryFile> RepositoryFiles { get; set; } = null!;

    public DbSet<Coworking.User> CoworkingUsers { get; set; } = null!;
    public DbSet<Coworking.Workspace> Workspaces { get; set; } = null!;
    public DbSet<Coworking.Booking> Bookings { get; set; } = null!;
    public DbSet<Coworking.Payment> Payments { get; set; } = null!;
    public DbSet<Coworking.ChatMessage> CoworkingChatMessages { get; set; } = null!;
    public DbSet<Coworking.AIQueryLog> AIQueryLogs { get; set; } = null!;

    public DbSet<Travel.User> TravelUsers { get; set; } = null!;
    public DbSet<Travel.Trip> Trips { get; set; } = null!;
    public DbSet<Travel.DayPlan> DayPlans { get; set; } = null!;
    public DbSet<Travel.Activity> Activities { get; set; } = null!;
    public DbSet<Travel.ChatMessage> TravelChatMessages { get; set; } = null!;
    public DbSet<Travel.Place> Places { get; set; } = null!;

    public DbSet<Support.User> SupportUsers { get; set; } = null!;
    public DbSet<Support.Ticket> Tickets { get; set; } = null!;
    public DbSet<Support.TicketSummary> TicketSummaries { get; set; } = null!;
    public DbSet<Support.KnowledgeBaseArticle> KnowledgeBaseArticles { get; set; } = null!;
    public DbSet<Support.ChatMessage> SupportChatMessages { get; set; } = null!;

    public DbSet<ResumeDomain.User> ResumeUsers { get; set; } = null!;
    public DbSet<ResumeDomain.Resume> Resumes { get; set; } = null!;
    public DbSet<ResumeDomain.JobPosting> JobPostings { get; set; } = null!;
    public DbSet<ResumeDomain.CoverLetter> CoverLetters { get; set; } = null!;
    public DbSet<ResumeDomain.ChatMessage> ResumeChatMessages { get; set; } = null!;

    public DbSet<DocumentSummarizer.User> DocumentUsers { get; set; } = null!;
    public DbSet<DocumentSummarizer.Document> Documents { get; set; } = null!;
    public DbSet<DocumentSummarizer.DocumentSummary> DocumentSummaries { get; set; } = null!;
    public DbSet<DocumentSummarizer.DocumentChunk> DocumentChunks { get; set; } = null!;
    public DbSet<DocumentSummarizer.ChatMessage> DocumentChatMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GitHub entities relationships
        modelBuilder.Entity<GitHubRepository>()
            .HasMany(r => r.Files)
            .WithOne(f => f.Repository)
            .HasForeignKey(f => f.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GitHubRepository>()
            .HasIndex(r => new { r.Owner, r.Name });

        modelBuilder.Entity<RepositoryFile>()
            .HasIndex(f => f.RepositoryId);

        modelBuilder.Entity<RepositoryFile>()
            .HasIndex(f => new { f.RepositoryId, f.FilePath })
            .IsUnique();
    }
}
