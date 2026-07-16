using Microsoft.EntityFrameworkCore;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Infrastructure.Persistence;

public class RiskDbContext : DbContext
{
    public RiskDbContext(DbContextOptions<RiskDbContext> options) : base(options)
    {
    }

    public DbSet<MarginMultiplier> MarginMultipliers { get; set; }
    public DbSet<ExitConfig> ExitConfigs { get; set; }
    public DbSet<VarAuditLog> VarAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarginMultiplier>(entity =>
        {
            entity.ToTable("margin_multipliers", "vyuh_config");
            entity.HasKey(e => e.StockId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<ExitConfig>(entity =>
        {
            entity.ToTable("exit_configs", "vyuh_config");
            entity.HasKey(e => e.StockId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<VarAuditLog>(entity =>
        {
            entity.ToTable("var_audit_logs", "vyuh_config");
            entity.HasKey(e => e.LogId);
        });
    }
}
