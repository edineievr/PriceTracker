using Pricetracker.Worker;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Jobs;
using PriceTracker.Worker.Services;
using PriceTracker.Worker.Strategies;

DotNetEnv.Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<MeliStrategy>();
builder.Services.AddScoped<KabumStrategy>();
builder.Services.AddHostedService<MonitoringJob>();
builder.Services.AddScoped<PriceComparisonService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var host = builder.Build();

var db = host.Services.GetRequiredService<Database>();
db.Initialize();

host.Run();
