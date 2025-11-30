using Microsoft.EntityFrameworkCore;
using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillDetail> BillDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ApiCallLog> ApiCallLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Subscriber configuration
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SubscriberNo).IsUnique();
            entity.Property(e => e.SubscriberNo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        // Bill configuration
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubscriberNo, e.Month }).IsUnique();
            entity.Property(e => e.SubscriberNo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Month).IsRequired().HasMaxLength(7);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Subscriber)
                .WithMany(s => s.Bills)
                .HasForeignKey(e => e.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BillDetail configuration
        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasMaxLength(50);
            
            entity.HasOne(e => e.Bill)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SubscriberNo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            
            entity.HasOne(e => e.Bill)
                .WithMany(b => b.Payments)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Subscriber)
                .WithMany(s => s.Payments)
                .HasForeignKey(e => e.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ApiCallLog configuration
        modelBuilder.Entity<ApiCallLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SubscriberNo).HasMaxLength(20);
            entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Method).IsRequired().HasMaxLength(10);
            
            entity.HasOne(e => e.Subscriber)
                .WithMany(s => s.ApiCallLogs)
                .HasForeignKey(e => e.SubscriberId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}

