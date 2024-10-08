using DiscountCode.Application;
using DiscountCode.Application.Options;
using DiscountCode.BackgroundServices;
using DiscountCode.BackgroundServices.Options;
using DiscountCode.Domain;
using DiscountCode.Domain.Generator;
using DiscountCode.Infrastructure;
using DiscountCode.Infrastructure.Persistance;
using DiscountCode.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<DiscountDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisConnection");
});

Console.WriteLine("REDIS CONNECTION STRING: " + configuration.GetConnectionString("RedisConnection"));

builder.Services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();
builder.Services.AddSingleton<ICodeGenerator, TimestampRandomCodeGenerator>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IDiscountCodeService, DiscountCodeService>();

builder.Services.Configure<DiscountCodeGenerationSettings>(
    builder.Configuration.GetSection("BackgroundService"));
builder.Services.Configure<DiscountCodeSettings>(
    builder.Configuration.GetSection("DiscountCode"));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddHostedService<CodeGenerationBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountCodeGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapGrpcReflectionService();
}

app.Run();