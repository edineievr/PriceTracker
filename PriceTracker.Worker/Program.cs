using Pricetracker.Worker;
using PriceTracker.Worker.Factories;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Services;
using PriceTracker.Worker.Strategies;

SQLitePCL.Batteries.Init();

DotNetEnv.Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
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
