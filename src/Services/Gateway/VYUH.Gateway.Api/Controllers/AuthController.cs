using Microsoft.AspNetCore.Mvc;
using VYUH.Gateway.Api.Helpers;

namespace VYUH.Gateway.Api.Controllers;

[ApiController]
[Route("api/v1/gateway/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        if (request.Username == "solanki_pm" && request.Password == "Password123")
        {
            var token = JwtTokenHelper.GenerateToken(request.Username);
            return Ok(new { token });
        }
        return Unauthorized("Invalid credentials");
    }
}

public class TokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
