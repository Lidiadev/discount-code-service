using DiscountCode.Application.Constants;
using DiscountCode.Application.Dtos;
using DiscountCode.Application.Options;
using DiscountCode.Domain;
using DiscountCode.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscountCode.Application;

public class DiscountCodeService : IDiscountCodeService
{
    private readonly IDiscountCodeRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DiscountCodeSettings _settings;
    private readonly ILogger<DiscountCodeService> _logger;

    public DiscountCodeService(
        ILogger<DiscountCodeService> logger,
        IDiscountCodeRepository repository,
        IDistributedCache cache,
        IDateTimeProvider dateTimeProvider,
        IOptions<DiscountCodeSettings> settings)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _dateTimeProvider = dateTimeProvider;
        _settings = settings.Value;
    }

    public async Task<GenerateCodesResponse> GenerateCodesAsync(GenerateCodeRequest request)
    {
        if (request.Count > _settings.MaxCodesPerRequest)
        {
            _logger
                .LogWarning("Requested code count {Count} exceeds maximum allowed {MaxCount}", request.Count, _settings.MaxCodesPerRequest);
            return GenerateCodesResponse.Failure(ErrorMessage.MaximumCountExceeded);
        }
        
        if (request.Count <= 0)
        {
            _logger.LogWarning("Requested code count {Count} is invalid", request.Count);
            return GenerateCodesResponse.Failure(ErrorMessage.InvalidCount);
        }
        
        if (!_settings.CodeGenerationLengths.Contains(request.Length))
        {
            _logger.LogWarning("Requested code request {Length} is not valid", request.Length);
            return GenerateCodesResponse.Failure(ErrorMessage.InvalidCodeLength);
        }

        try
        {
            var availableCodes = await _repository.GenerateCodesAsync(request.Count, request.Length);
            var codes = availableCodes.Select(ac => ac.Code).ToList();

            if (codes.Count == 0)
            {
                _logger.LogWarning("No codes have been generated");
                return GenerateCodesResponse.Failure(ErrorMessage.ErrorNoCodeGenerated);
            }
            
            return GenerateCodesResponse.Success(codes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating discount codes");
            return GenerateCodesResponse.Failure(ErrorMessage.GenerationFailed);
        }
    }

    public async Task<UseCodeResult> UseCodeAsync(UseCodeRequest request)
    {
        try
        {
            Domain.Entities.DiscountCode.Validate(request.Code, _settings.UsedCodeLength);  
            
            var cacheResult = await _cache.GetStringAsync(request.Code);
            if (cacheResult == CacheValue.Used)
            {
                return UseCodeResult.Failure(UseCodeStatus.AlreadyUsed, ErrorMessage.CodeAlreadyUsed);
            }

            var discountCode = await _repository.GetDiscountCodeAsync(request.Code);

            if (discountCode == null)
            {
                await _cache.SetStringAsync(request.Code, CacheValue.NotFound, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.NotFoundCacheExpirationMinutes)
                });

                return UseCodeResult.Failure(UseCodeStatus.NotFound, ErrorMessage.CodeNotFound);
            }

            discountCode.MarkAsUsed(_dateTimeProvider.UtcNow);

            if (await _repository.MarkAsModifiedAsync(discountCode))
            {
                await _cache.SetStringAsync(request.Code, CacheValue.Used, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_settings.UsedCodeCacheExpirationDays)
                });

                _logger.LogInformation("Successfully used discount code: {Code}", request.Code);
                return UseCodeResult.Success(UseCodeStatus.Success, "Discount code successfully used");
            }

            _logger.LogWarning("Concurrency error while using discount code: {Code}", request.Code);
            return UseCodeResult.Failure(UseCodeStatus.ConcurrencyError, ErrorMessage.ConcurrencyError);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid discount code: {Code}", request.Code);
            return UseCodeResult.Failure(UseCodeStatus.Invalid, ErrorMessage.CodeInvalid);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Attempted to use already used discount code: {Code}", request.Code);
            return UseCodeResult.Failure(UseCodeStatus.AlreadyUsed, ErrorMessage.CodeAlreadyUsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using discount code: {Code}", request.Code);
            return UseCodeResult.Failure(UseCodeStatus.Error, ErrorMessage.ErrorGeneratingCodes);
        }
    }

    public async Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes)
    {
        await _repository.AddAvailableCodesAsync(codes);
    }
}