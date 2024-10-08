﻿using DiscountCode.BackgroundServices.Options;
using DiscountCode.Domain;
using DiscountCode.Domain.Entities;
using DiscountCode.Domain.Generator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscountCode.BackgroundServices;

// public class CodeGenerationBackgroundService : IHostedService
// {
//     private readonly ILogger<CodeGenerationBackgroundService> _logger;
//     private readonly IServiceProvider _serviceProvider;
//     private readonly ICodeGenerator _codeGenerator;
//     private readonly IDateTimeProvider _dateTimeProvider;
//     private readonly DiscountCodeGenerationSettings _settings;
//     private CancellationTokenSource _cancellationTokenSource;
//
//     public CodeGenerationBackgroundService(
//         ILogger<CodeGenerationBackgroundService> logger,
//         IServiceProvider serviceProvider, 
//         ICodeGenerator codeGenerator,
//         IDateTimeProvider dateTimeProvider,
//         IOptions<DiscountCodeGenerationSettings> options)
//     {
//         _logger = logger;
//         _serviceProvider = serviceProvider;
//         _codeGenerator = codeGenerator;
//         _dateTimeProvider = dateTimeProvider;
//         _settings = options.Value;
//     }
//
//     
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
//         await ExecuteAsync(_cancellationTokenSource.Token);
//     }
//
//     public Task StopAsync(CancellationToken cancellationToken)
//     {
//         _cancellationTokenSource.Cancel();
//         return Task.CompletedTask;
//     }
//
//     private async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // TODO: is it possible to overflow the table????
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 // using var scope = _serviceProvider.CreateScope();
//                 // var repository = scope.ServiceProvider.GetRequiredService<IDiscountCodeRepository>();
//                 //
//                 // var codes = _codeGenerator
//                 //     .GenerateCodes(_settings.CodeGenerationBatchSize)
//                 //     .Select(code => AvailableDiscountCode.Create(code, _dateTimeProvider.UtcNow));
//                 //
//                 // await repository.AddAvailableCodesAsync(codes);
//                 _logger.LogInformation("Generated {Count} new available discount codes", _settings.CodeGenerationBatchSize);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error generating discount codes");
//             }
//             
//             _logger.LogInformation("Waiting {Count}", _settings.CodeGenerationIntervalSeconds);
//             await Task.Delay(TimeSpan.FromSeconds(_settings.CodeGenerationIntervalSeconds), stoppingToken);
//         }
//     }
// }

public class CodeGenerationBackgroundService : BackgroundService
{
    private readonly ILogger<CodeGenerationBackgroundService> _logger;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DiscountCodeGenerationSettings _settings;

    private IServiceProvider Services { get; }

    public CodeGenerationBackgroundService(
        ILogger<CodeGenerationBackgroundService> logger,
        IServiceProvider serviceProvider,
        ICodeGenerator codeGenerator,
        IDateTimeProvider dateTimeProvider,
        IOptions<DiscountCodeGenerationSettings> options)
    {
        _logger = logger;
        _codeGenerator = codeGenerator;
        _dateTimeProvider = dateTimeProvider;
        _settings = options.Value;
        Services = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Code Generation Hosted Service running.");

        await DoWorkAsync();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_settings.CodeGenerationIntervalSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWorkAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Code Generation Service is stopping.");
        }
    }
    
    private async Task DoWorkAsync()
    {
        try
        {
            using var scope = Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDiscountCodeRepository>();
            
            var codes = _codeGenerator
                .GenerateCodes(_settings.CodeGenerationBatchSize)
                .Select(code => AvailableDiscountCode.Create(code, _dateTimeProvider.UtcNow))
                .ToList();
            
            await repository.AddAvailableCodesAsync(codes);
            _logger.LogInformation("Generated {Count} new available discount codes", _settings.CodeGenerationBatchSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating discount codes");
        }
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
     {
         _logger.LogInformation("Code Generation Hosted Service is starting.");

         await base.StartAsync(cancellationToken);
     }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Code Generation Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}