using System;

namespace VYUH.Optimizer.Application.Services;

public static class BsmMath
{
    // Abramowitz and Stegun rational approximation of standard normal cumulative distribution function (CDF)
    public static double Phi(double x)
    {
        // Constants for Abramowitz and Stegun approximation
        const double p = 0.2316419;
        const double a1 = 0.319381530;
        const double a2 = -0.356563782;
        const double a3 = 1.781477937;
        const double a4 = -1.821255978;
        const double a5 = 1.330274429;

        var sign = x < 0 ? -1.0 : 1.0;
        var absX = Math.Abs(x);

        var t = 1.0 / (1.0 + p * absX);
        var poly = t * (a1 + t * (a2 + t * (a3 + t * (a4 + t * a5))));
        var cdf = 1.0 - (1.0 / Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-0.5 * absX * absX) * poly;

        return sign < 0 ? 1.0 - cdf : cdf;
    }

    public static double CalculateD2(double spot, double strike, double r, double iv, double t)
    {
        if (t <= 0 || iv <= 0) return 0.0;
        var d1 = (Math.Log(spot / strike) + (r + 0.5 * iv * iv) * t) / (iv * Math.Sqrt(t));
        return d1 - iv * Math.Sqrt(t);
    }
    public static double CalculateTouchProbability(double spot, double strike, double r, double iv, double t)
    {
        if (t <= 0 || iv <= 0) return 0.0;
        
        var mu = r - 0.5 * iv * iv;
        var sigmaSqrtT = iv * Math.Sqrt(t);
        var twoMuOverSigma2 = 2.0 * mu / (iv * iv);

        var y1 = (Math.Log(spot / strike) + mu * t) / sigmaSqrtT;
        var y2 = (Math.Log(spot / strike) - mu * t) / sigmaSqrtT;

        if (strike > spot)
        {
            var p1 = Phi(y1);
            var p2 = Phi(y2);
            var term = Math.Pow(strike / spot, twoMuOverSigma2) * p2;
            return Math.Clamp(p1 + term, 0.0, 1.0);
        }
        else
        {
            var p1 = Phi(-y1);
            var p2 = Phi(-y2);
            var term = Math.Pow(strike / spot, twoMuOverSigma2) * p2;
            return Math.Clamp(p1 + term, 0.0, 1.0);
        }
    }
}
