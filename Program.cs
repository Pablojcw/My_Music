using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sound.Api.Middleware;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Application.Services;
using Scalar.AspNetCore;
using SoundSpace.Infrastructure.Repositories;
using SoundSpace.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Logging.AddFilter("System.Net.Http.HttpClient.YouTubeApi", LogLevel.Warning);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("youtube-search", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "local",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Limite de pesquisas atingido",
            Detail = "Aguarde um minuto antes de fazer novas pesquisas."
        }, cancellationToken);
    };
});
builder.Services.AddExceptionHandler<InvalidMusicReferenceExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidAlbumReferenceExceptionHandler>();
builder.Services.AddExceptionHandler<LibraryPlaylistExceptionHandler>();
builder.Services.AddExceptionHandler<YouTubeApiExceptionHandler>();

builder.Services.AddDbContext<SoundSpaceDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddHttpClient("YouTubeApi", client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddScoped<IYouTubeMusicService>(serviceProvider =>
    new YouTubeMusicService(
        serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("YouTubeApi"),
        builder.Configuration["YouTube:ApiKey"],
        serviceProvider.GetRequiredService<IMusicRepository>()));
var app = builder.Build();


app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
