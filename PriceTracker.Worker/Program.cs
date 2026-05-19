using Microsoft.AspNetCore.Builder;
using Pricetracker.Worker;
using PriceTracker.Worker.Factories;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Services;
using PriceTracker.Worker.Strategies;
using Serilog;
using System.Text.Json.Serialization;

SQLitePCL.Batteries.Init();

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:\\Users\\edine\\source\\repos\\PriceTracker\\Logs\\PriceTracker.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<PichauStrategy>();
builder.Services.AddScoped<KabumStrategy>();
builder.Services.AddScoped<TerabyteStrategy>();
builder.Services.AddScoped<PriceComparisonService>();
builder.Services.AddTransient<PriceTrackerFactory>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<Database>(provider => new Database("Data Source=C:\\Users\\edine\\source\\storage\\price_tracker.db"));


var app = builder.Build();

var db = app.Services.GetRequiredService<Database>();

await db.InitializeAsync(app.Lifetime.ApplicationStopped);

app.MapControllers();

app.Run();

try
{

}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada por falha");
}
finally
{
    Log.CloseAndFlush();
}


