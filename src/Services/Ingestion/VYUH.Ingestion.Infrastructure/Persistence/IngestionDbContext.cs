using Microsoft.EntityFrameworkCore;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Infrastructure.Persistence;

public class IngestionDbContext : DbContext
{
    public IngestionDbContext(DbContextOptions<IngestionDbContext> options) : base(options)
    {
    }

    public DbSet<UnderlyingStock> UnderlyingStocks { get; set; }
    public DbSet<OptionLiquidityLog> OptionLiquidityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnderlyingStock>(entity =>
        {
            entity.ToTable("underlying_stocks", "vyuh_config");
            entity.HasKey(e => e.StockId);
            entity.Property(e => e.StockId).HasMaxLength(20);
            entity.Property(e => e.StockName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Sector).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Industry).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<OptionLiquidityLog>(entity =>
        {
            entity.ToTable("option_liquidity_logs", "vyuh_analytics");
            entity.HasKey(e => e.LogId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
            entity.Property(e => e.OptionType).HasMaxLength(2).IsRequired();
            entity.Property(e => e.ExpiryDate).HasMaxLength(20).IsRequired();
        });
    }
}
