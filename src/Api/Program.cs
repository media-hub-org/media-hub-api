using MediaHub.Api.BackgroundServices;
using MediaHub.Api.Middleware;
using MediaHub.Application.Services;
using MediaHub.Infrastructure.Music;
using MediaHub.Infrastructure.Persistence;
using MediaHub.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<R2StorageOptions>(builder.Configuration.GetSection("R2"));
builder.Services.AddSingleton<IStorageService, R2StorageService>();

builder.Services.AddScoped<LocalUploadSourceProvider>();
builder.Services.AddSingleton<YouTubeSourceProvider>();

builder.Services.AddScoped<IMusicTrackRepository, MusicTrackRepository>();
builder.Services.AddScoped<IMusicImportJobRepository, MusicImportJobRepository>();
builder.Services.AddScoped<MusicIngestionService>();

builder.Services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<DeviceTokenMiddleware>();
app.MapControllers();

app.Run();