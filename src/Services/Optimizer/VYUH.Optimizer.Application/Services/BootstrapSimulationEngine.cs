using System;
using System.Collections.Generic;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Services;

public class BootstrapSimulationEngine
{
    private readonly Random _random = new();

    public ProbabilityResult RunSimulation(
        string stockId, 
        double spotPrice, 
        double strikePrice, 
        string expiryDate, 
        List<double> logReturns, 
        int numPaths = 5000)
    {
        DateTime.TryParse(expiryDate, out var expiryDateTime);
        var daysToExpiry = Math.Max(1, (expiryDateTime - DateTime.UtcNow).Days);

        if (logReturns == null || logReturns.Count == 0)
        {
            logReturns = new List<double> { 0.0 };
        }

        var countsAbove = 0;
        var countsBelow = 0;

        for (int i = 0; i < numPaths; i++)
        {
            var cumReturn = 0.0;
            for (int t = 0; t < daysToExpiry; t++)
            {
                var index = _random.Next(logReturns.Count);
                cumReturn += logReturns[index];
            }
            
            var endingPrice = spotPrice * Math.Exp(cumReturn);
            if (endingPrice > strikePrice)
            {
                countsAbove++;
            }
            else
            {
                countsBelow++;
            }
        }

        var probAbove = (double)countsAbove / numPaths;
        var probBelow = (double)countsBelow / numPaths;

        return new ProbabilityResult
        {
            StockId = stockId,
            SpotPrice = spotPrice,
            ExpiryDate = expiryDate,
            StrikePrice = strikePrice,
            ProbAbove = probAbove,
            ProbBelow = probBelow,
            NumberOfSimulations = numPaths,
            Timestamp = DateTime.UtcNow
        };
    }
}
