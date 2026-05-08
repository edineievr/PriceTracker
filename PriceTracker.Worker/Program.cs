using Pricetracker.Worker;
using PriceTracker.Worker.Factories;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Services;
using PriceTracker.Worker.Strategies;
using Serilog;

SQLitePCL.Batteries.Init();

DotNetEnv.Env.Load();

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("D:\\Projects\\edineievr\\PriceTracker\\PriceTracker.Worker\\Logs\\pricetracker.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHostedService<Worker>();
builder.Services.AddSerilog();
builder.Services.AddScoped<MeliStrategy>();
builder.Services.AddScoped<KabumStrategy>();
builder.Services.AddScoped<PriceComparisonService>();
builder.Services.AddTransient<PriceTrackerFactory>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<Database>(provider => new Database("Data Source=D:\\Projects\\edineievr\\PriceTracker\\PriceTracker.Worker\\price_tracker.db"));

var host = builder.Build();

var db = host.Services.GetRequiredService<Database>();

await db.InitializeAsync();

host.Run();
