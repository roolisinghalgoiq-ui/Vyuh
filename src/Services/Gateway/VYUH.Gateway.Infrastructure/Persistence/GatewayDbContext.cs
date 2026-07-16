using Microsoft.EntityFrameworkCore;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Infrastructure.Persistence;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
    {
    }

    public DbSet<OrderExecutionLog> OrderExecutionLogs { get; set; }
    public DbSet<PositionReconciliationMismatch> PositionReconciliationMismatches { get; set; }
    public DbSet<UserAuditLog> UserAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderExecutionLog>(entity =>
        {
            entity.ToTable("order_execution_logs", "vyuh_config");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
            entity.Property(e => e.OptionType).HasMaxLength(5).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<PositionReconciliationMismatch>(entity =>
        {
            entity.ToTable("position_reconciliation_mismatches", "vyuh_config");
            entity.HasKey(e => e.MismatchId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
            entity.Property(e => e.OptionType).HasMaxLength(5).IsRequired();
            entity.Property(e => e.DiscrepancyType).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<UserAuditLog>(entity =>
        {
            entity.ToTable("user_audit_logs", "vyuh_config");
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(50).IsRequired();
            entity.Property(e => e.RequestDetails).IsRequired();
        });
    }
}
