using DiscountCode.Application.Constants;
using DiscountCode.Application.Dtos;
using DiscountCode.Application.Options;
using DiscountCode.Domain;
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

    public async Task<GenerateCodesResponse> GenerateCodesAsync(int count)
    {
        var maxGenerationLimit = _settings.MaxCodesPerRequest;
        
        if (count > maxGenerationLimit)
        {
            _logger
                .LogWarning("Requested code count {Count} exceeds maximum allowed {MaxCount}", count, _settings.MaxCodesPerRequest);
            return GenerateCodesResponse.Failure(ErrorMessage.MaximumCountExceeded);
        }

        try
        {
            var availableCodes = await _repository.GetAvailableCodesAsync(count);
            var codesToMove = availableCodes.Select(ac => ac.Code).ToList();
            
            if (await _repository.MoveToDiscountCodesAsync(codesToMove))
            {
                return GenerateCodesResponse.Success(codesToMove);
            }

            _logger.LogError("Failed to move {Count} codes from available to discount codes", codesToMove.Count);
            return GenerateCodesResponse.Failure(ErrorMessage.ErrorMovingCodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating discount codes");
            return GenerateCodesResponse.Failure(ErrorMessage.GenerationFailed);
        }
    }

    public async Task<UseCodeResult> UseCodeAsync(string code)
    {
        try
        {
            Domain.Entities.DiscountCode.Validate(code, _settings.CodeLength);  
            
            var cacheResult = await _cache.GetStringAsync(code);
            if (cacheResult == "used")
            {
                return UseCodeResult.Failure(UseCodeStatus.AlreadyUsed, ErrorMessage.CodeAlreadyUsed);
            }

            var discountCode = await _repository.GetDiscountCodeAsync(code);

            if (discountCode == null)
            {
                await _cache.SetStringAsync(code, "not_found", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.NotFoundCacheExpirationMinutes)
                });

                return UseCodeResult.Failure(UseCodeStatus.NotFound, ErrorMessage.CodeNotFound);
            }

            discountCode.MarkAsUsed(_dateTimeProvider.UtcNow);

            if (await _repository.MarkAsModifiedAsync(discountCode))
            {
                await _cache.SetStringAsync(code, "used", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_settings.UsedCodeCacheExpirationDays)
                });

                _logger.LogInformation("Successfully used discount code: {Code}", code);
                return UseCodeResult.Success(UseCodeStatus.Success, "Discount code successfully used");
            }

            _logger.LogWarning("Concurrency error while using discount code: {Code}", code);
            return UseCodeResult.Failure(UseCodeStatus.ConcurrencyError, ErrorMessage.ConcurrencyError);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid discount code: {Code}", code);
            return UseCodeResult.Failure(UseCodeStatus.Invalid, ErrorMessage.CodeInvalid);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Attempted to use already used discount code: {Code}", code);
            return UseCodeResult.Failure(UseCodeStatus.AlreadyUsed, ErrorMessage.CodeAlreadyUsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using discount code: {Code}", code);
            return UseCodeResult.Failure(UseCodeStatus.Error, ErrorMessage.ErrorGeneratingCodes);
        }
    }
}