using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Reference your services & models
using HumanCentricAttackPath.Services;
using HumanCentricAttackPath.Models;

using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// 1) Register controllers so that [ApiController]‐decorated classes get routed
builder.Services.AddControllers();

// 2) (Optional) If you want OpenAPI/Swagger, you can do this instead:
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Sample endpoint; leave it or comment it out as you like
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// ====== Test code block ======
{
    var service = new AttackGraphService();
    DemoData demo = service.LoadData();

    Console.WriteLine($"Loaded {demo.persons.Count} persons from JSON:");
    foreach (var p in demo.persons)
    {
        Console.WriteLine($"  - {p.name} (trained? {p.has_phish_training})");
    }

    Console.WriteLine($"Loaded {demo.locations.Count} locations from JSON:");
    foreach (var loc in demo.locations)
    {
        Console.WriteLine($"  - {loc.name} (badge reader? {loc.has_badge_reader})");
    }

    Console.WriteLine($"Loaded {demo.assets.Count} assets from JSON:");
    foreach (var a in demo.assets)
    {
        Console.WriteLine($"  - {a.name} (critical? {a.is_critical})");
    }
}
// =============================

 // 3) Map controllers so that routes in AttackController are exposed
app.MapControllers();

app.UseCors("AllowAll");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
