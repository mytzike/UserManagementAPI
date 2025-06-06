using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Add support for controllers

// ✅ Step 1: Configure Middleware Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ Use JWT authentication without an external provider (local development)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Allows local testing
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // No external authentication needed
            ValidateAudience = false, // Skip audience validation
            ValidateLifetime = true, // Check token expiration
            ValidateIssuerSigningKey = true, // Verify signature
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
        };
    });

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseHsts();

// ✅ Step 2: Register Middleware (Correct Order)
app.UseAuthentication(); // Ensure security before requests reach endpoints
app.UseMiddleware<ErrorHandlingMiddleware>(); // Catches unhandled exceptions
//app.UseMiddleware<AuthenticationMiddleware>(); // Manually validates JWT tokens
app.UseMiddleware<RequestLoggingMiddleware>(); // Logs requests & responses
app.UseAuthorization(); // Enables role-based access control

app.UseCors("AllowAll"); // Allows API access from other applications

app.MapControllers();
app.Run();
