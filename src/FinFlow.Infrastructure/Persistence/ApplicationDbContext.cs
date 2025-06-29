using System.Security.Claims;
using FinFlow.Application.Contracts.Persistence;
using FinFlow.Domain;
using FinFlow.Domain.Audit;
using FinFlow.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options), IApplicationDbContext
{
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var username = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "system";

        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = username;
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = username;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = username;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Wallet> Wallets => Set<Wallet>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure all ID properties as strings
        builder.Entity<User>()
            .Property(u => u.Id)
            .HasColumnType("text");

        builder.Entity<Category>()
            .Property(c => c.Id)
            .HasColumnType("text");

        builder.Entity<Transaction>()
            .Property(t => t.Id)
            .HasColumnType("text");

        builder.Entity<Wallet>()
            .Property(w => w.Id)
            .HasColumnType("text");

        builder.Entity<Category>()
            .HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Category>()
            .Property(c => c.Type)
            .HasConversion<string>();

        builder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();

        builder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Wallet>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wallet>()
            .Property(w => w.Type)
            .HasConversion<string>();

        builder.Entity<Wallet>()
            .Property(w => w.Currency)
            .HasConversion<string>();

        builder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasPrecision(18, 2);

        builder.Entity<Transaction>()
            .HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        base.OnModelCreating(builder);
    }
}