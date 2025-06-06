using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Missing authentication token.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("{\"error\": \"Unauthorized. Token required.\"}");
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                _logger.LogWarning("Invalid authentication token.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("{\"error\": \"Unauthorized. Invalid token.\"}");
                return;
            }

            await _next(context); // Continue request processing if token is valid
        }
        catch
        {
            _logger.LogError("Error processing authentication token.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("{\"error\": \"Unauthorized. Token validation failed.\"}");
        }
    }
}
