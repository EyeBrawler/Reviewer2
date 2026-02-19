using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reviewer2.Data.Models;

namespace Reviewer2.Data.Context;

/// <inheritdoc/>
public class ApplicationContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <inheritdoc/>>
    public ApplicationContext(
        DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Represents all submitted papers within the system.
    /// 
    /// Papers are the root aggregate for the submission domain and are
    /// associated with authors, files, review assignments, and reviews.
    /// </summary>
    public DbSet<Paper> Papers => Set<Paper>();

    /// <summary>
    /// Represents all authors associated with submitted papers.
    /// 
    /// Authors are owned by a Paper and define authorship order,
    /// institutional affiliation, and corresponding/presenter roles.
    /// </summary>
    public DbSet<Author> Authors => Set<Author>();

    /// <summary>
    /// Represents uploaded files associated with papers.
    /// 
    /// This includes initial submissions, camera-ready versions,
    /// copyright forms, and other file types defined by <see cref="FileType"/>.
    /// </summary>
    public DbSet<PaperFile> PaperFiles => Set<PaperFile>();

    /// <summary>
    /// Represents assignments of reviewers to papers.
    /// 
    /// A ReviewAssignment links a reviewer to a paper and tracks
    /// review workflow status. Each assignment may result in a single Review.
    /// </summary>
    public DbSet<ReviewAssignment> ReviewAssignments => Set<ReviewAssignment>();

    /// <summary>
    /// Represents completed or in-progress reviews submitted by reviewers.
    /// 
    /// Reviews are created in response to a ReviewAssignment and contain
    /// both structured scoring fields and flexible JSON-based content.
    /// </summary>
    public DbSet<Review> Reviews => Set<Review>();

    /// <summary>
    /// Represents versioned templates that define the structure
    /// and validation schema for reviews.
    /// 
    /// Templates determine the JSON schema used for review content
    /// and allow historical consistency through versioning.
    /// </summary>
    public DbSet<ReviewTemplate> ReviewTemplates => Set<ReviewTemplate>();
    
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigurePaper(builder);
        ConfigureAuthor(builder);
        ConfigureReviewAssignment(builder);
        ConfigureReview(builder);
    }

    private static void ConfigureAuthor(ModelBuilder builder)
    {
        builder.Entity<Author>()
            .HasIndex(a => new { a.PaperId, a.AuthorOrder })
            .IsUnique();

        builder.Entity<Author>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureReviewAssignment(ModelBuilder builder)
    {
        builder.Entity<ReviewAssignment>()
            .HasIndex(r => new { r.PaperId, r.ReviewerId })
            .IsUnique();

        builder.Entity<ReviewAssignment>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureReview(ModelBuilder builder)
    {
        builder.Entity<Review>()
            .HasOne(r => r.ReviewAssignment)
            .WithOne(a => a.Review)
            .HasForeignKey<Review>(r => r.ReviewAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private static void ConfigurePaper(ModelBuilder builder)
    {
        builder.Entity<Paper>()
            .HasOne(p => p.Submitter)
            .WithMany()
            .HasForeignKey(p => p.SubmitterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Paper>()
            .HasMany(p => p.Authors)
            .WithOne(a => a.Paper)
            .HasForeignKey(a => a.PaperId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}