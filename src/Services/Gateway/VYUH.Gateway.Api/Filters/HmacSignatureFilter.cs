using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VYUH.Gateway.Api.Filters;

public class HmacSignatureFilter : IAsyncActionFilter
{
    private const string SignatureHeader = "X-Client-Signature";
    private const string SharedSecret = "SuperSecret123";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue(SignatureHeader, out var signature))
        {
            context.Result = new BadRequestObjectResult($"Missing required header '{SignatureHeader}'.");
            return;
        }

        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();

        if (signature.ToString().ToLowerInvariant() != computedSignature)
        {
            context.Result = new UnauthorizedObjectResult("Invalid client signature.");
            return;
        }

        await next();
    }
}
