using MediaHub.Application.Services;
using MediaHub.Domain.Entities;
using MediaHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.Api.Controllers;

[ApiController]
[Route("devices")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public DevicesController(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public record CreateDeviceRequest(string Name, string MasterKey);
    public record CreateDeviceResponse(Guid Id, string Name, string Token);

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request)
    {
        var expectedMasterKey = _configuration["MasterKey"];

        if (string.IsNullOrEmpty(expectedMasterKey) || request.MasterKey != expectedMasterKey)
        {
            return Unauthorized("Invalid master key.");
        }

        var token = TokenHasher.GenerateToken();
        var tokenHash = TokenHasher.Hash(token);

        var device = new Device
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Devices.Add(device);
        await _db.SaveChangesAsync();

        return Ok(new CreateDeviceResponse(device.Id, device.Name, token));
    }

    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var device = await _db.Devices.FindAsync(id);

        if (device is null)
        {
            return NotFound();
        }

        device.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}