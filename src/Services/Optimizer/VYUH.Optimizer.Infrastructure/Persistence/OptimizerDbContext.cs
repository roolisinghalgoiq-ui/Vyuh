using Microsoft.EntityFrameworkCore;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class OptimizerDbContext : DbContext
{
    public OptimizerDbContext(DbContextOptions<OptimizerDbContext> options) : base(options)
    {
    }

    public DbSet<StockScoreArchive> StockScores { get; set; }
    public DbSet<FilteringThreshold> FilteringThresholds { get; set; }
    public DbSet<StrikeSelectionConfig> StrikeSelectionConfigs { get; set; }
    public DbSet<AiParameterProposal> AiParameterProposals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockScoreArchive>(entity =>
        {
            entity.ToTable("stock_scores_archive", "vyuh_analytics");
            entity.HasKey(e => e.ScoreId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
            entity.Property(e => e.StrategySelected).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<FilteringThreshold>(entity =>
        {
            entity.ToTable("filtering_thresholds", "vyuh_config");
            entity.HasKey(e => e.FilterId);
            entity.Property(e => e.FilterId).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<StrikeSelectionConfig>(entity =>
        {
            entity.ToTable("strike_selection_configs", "vyuh_config");
            entity.HasKey(e => e.SettingId);
            entity.Property(e => e.SettingId).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<AiParameterProposal>(entity =>
        {
            entity.ToTable("ai_parameter_proposals", "vyuh_config");
            entity.HasKey(e => e.ProposalId);
            entity.Property(e => e.StockId).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ParameterName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CurrentValue).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ProposedValue).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
        });
    }
}
