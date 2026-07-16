using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application.Queries;

public record EstimateMarginQuery(
    string StockId, 
    double SpotPrice, 
    double StrikePrice, 
    string OptionType, 
    string ExpiryDate, 
    double ImpliedVolatility, 
    int LotSize) : IRequest<MarginEstimationResult>;

public class EstimateMarginQueryHandler : IRequestHandler<EstimateMarginQuery, MarginEstimationResult>
{
    private readonly IMarginMultiplierRepository _repository;

    public EstimateMarginQueryHandler(IMarginMultiplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<MarginEstimationResult> Handle(EstimateMarginQuery request, CancellationToken cancellationToken)
    {
        // Fetch multipliers configs
        var config = await _repository.GetMultiplierAsync(request.StockId);
        var multiplier = config?.MultiplierValue ?? 1.0;
        var expPct = config?.ExposureMarginPct ?? 0.03;
        var volShift = config?.VolShiftPct ?? 0.10;

        var t = 15.0 / 365.0; // Assume 15 days to expiry for margin estimation runs
        var r = 0.07; // standard 7% interest rate

        // Compute current option premium P0 (using BSM pricing helper)
        var p0 = CalculateBsmOptionPrice(request.SpotPrice, request.StrikePrice, r, request.ImpliedVolatility, t, request.OptionType);

        // Price range shock: 3 * sigma * Spot * sqrt(T)
        var priceRange = 3.0 * request.ImpliedVolatility * request.SpotPrice * Math.Sqrt(t);
        
        // 16 pricing scenarios (combinations of Spot price shifts and IV shifts)
        double[] spotShifts = { 
            0.0, 
            priceRange / 3.0, -priceRange / 3.0, 
            2.0 * priceRange / 3.0, -2.0 * priceRange / 3.0, 
            priceRange, -priceRange, 
            2.0 * priceRange, -2.0 * priceRange 
        };
        double[] volShifts = { volShift, -volShift };

        double maxLoss = 0.0;

        foreach (var shift in spotShifts)
        {
            var isExtreme = Math.Abs(shift) >= (2.0 * priceRange);
            var shiftedSpot = Math.Max(1.0, request.SpotPrice + shift);

            foreach (var vol in volShifts)
            {
                var shiftedVol = Math.Max(0.01, request.ImpliedVolatility * (1.0 + vol));
                var pNew = CalculateBsmOptionPrice(shiftedSpot, request.StrikePrice, r, shiftedVol, t, request.OptionType);

                // Loss from short options writing position
                var loss = (pNew - p0) * request.LotSize;

                if (isExtreme)
                {
                    loss *= 0.35; // 35% extreme move coverage
                }

                if (loss > maxLoss)
                {
                    maxLoss = loss;
                }
            }
        }

        // Add minimum baseline floor (e.g. 10% of gross contract value) to prevent under-margining deep OTM positions
        var minFloor = request.SpotPrice * request.LotSize * 0.10;
        var spanMargin = Math.Max(maxLoss, minFloor) * multiplier;

        // Exposure Margin: gross notional * exposure_margin_pct
        var exposureMargin = request.SpotPrice * request.LotSize * expPct * multiplier;

        return new MarginEstimationResult
        {
            StockId = request.StockId,
            ExpiryDate = request.ExpiryDate,
            SpotPrice = request.SpotPrice,
            StrikePrice = request.StrikePrice,
            OptionType = request.OptionType,
            SpanMargin = Math.Round(spanMargin, 2),
            ExposureMargin = Math.Round(exposureMargin, 2),
            TotalMargin = Math.Round(spanMargin + exposureMargin, 2),
            Timestamp = DateTime.UtcNow
        };
    }

    private double Phi(double x)
    {
        double a1 = 0.319381530;
        double a2 = -0.356563782;
        double a3 = 1.781477937;
        double a4 = -1.821255978;
        double a5 = 1.330274429;
        double p = 0.2316419;
        double c = 0.39894228;

        if (x >= 0.0)
        {
            double k = 1.0 / (1.0 + p * x);
            return 1.0 - c * Math.Exp(-x * x / 2.0) * k *
                (a1 + k * (a2 + k * (a3 + k * (a4 + k * a5))));
        }
        else
        {
            return 1.0 - Phi(-x);
        }
    }

    private double CalculateBsmOptionPrice(double spot, double strike, double r, double iv, double t, string type)
    {
        if (t <= 0 || iv <= 0)
        {
            return type.Equals("CE", StringComparison.OrdinalIgnoreCase)
                ? Math.Max(0.0, spot - strike)
                : Math.Max(0.0, strike - spot);
        }

        var d1 = (Math.Log(spot / strike) + (r + 0.5 * iv * iv) * t) / (iv * Math.Sqrt(t));
        var d2 = d1 - iv * Math.Sqrt(t);

        if (type.Equals("CE", StringComparison.OrdinalIgnoreCase))
        {
            return spot * Phi(d1) - strike * Math.Exp(-r * t) * Phi(d2);
        }
        else
        {
            return strike * Math.Exp(-r * t) * Phi(-d2) - spot * Phi(-d1);
        }
    }
}
