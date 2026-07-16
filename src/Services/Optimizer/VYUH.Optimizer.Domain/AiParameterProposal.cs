using System;

namespace VYUH.Optimizer.Domain;

public class AiParameterProposal
{
    public Guid ProposalId { get; set; }
    public string StockId { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string CurrentValue { get; set; } = string.Empty;
    public string ProposedValue { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}
