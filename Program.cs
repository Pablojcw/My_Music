using Microsoft.EntityFrameworkCore;
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
builder.Services.AddExceptionHandler<InvalidMusicReferenceExceptionHandler>();

builder.Services.AddDbContext<SoundSpaceDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<IMusicService, MusicService>();
var app = builder.Build();


app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
