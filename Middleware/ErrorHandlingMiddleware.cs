using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Continue processing
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled Exception: {ex.Message}"); // Log the error

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json"; // Set response format to JSON
            
            var errorResponse = new { error = "Internal server error." }; // Standardized response
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse)); // Send JSON response
        }
    }
}
