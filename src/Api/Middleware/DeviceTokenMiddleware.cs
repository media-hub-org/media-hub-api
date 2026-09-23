using MediaHub.Application.Services;
using MediaHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MediaHub.Api.Middleware;

public class DeviceTokenMiddleware
{
    private readonly RequestDelegate _next;

    public DeviceTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

        if (allowAnonymous)
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid Authorization header.");
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();
        var tokenHash = TokenHasher.Hash(token);

        var device = await db.Devices
            .FirstOrDefaultAsync(d => d.TokenHash == tokenHash && d.IsActive);

        if (device is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or revoked token.");
            return;
        }

        device.LastAccessAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        context.Items["Device"] = device;

        await _next(context);
    }
}