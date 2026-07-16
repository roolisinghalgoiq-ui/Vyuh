namespace VYUH.Ingestion.Domain;

public class OptionContract
{
    public double StrikePrice { get; set; }
    public OptionType Type { get; set; }
    public double Bid { get; set; }
    public double Ask { get; set; }
    public long OpenInterest { get; set; }
    public long Volume { get; set; }
    public double Delta { get; set; }
    public double Gamma { get; set; }
    public double Vega { get; set; }
    public double Theta { get; set; }
    public double ImpliedVolatility { get; set; }
    public double LiquidityScore { get; set; }
}
