using DiscountCode.Application;
using DiscountService;
using Grpc.Core;

namespace DiscountCode.Service.Services;

public class DiscountCodeGrpcService : DiscountService.DiscountService.DiscountServiceBase
{
    private readonly IDiscountCodeService _discountCodeService;
    private readonly ILogger<DiscountCodeGrpcService> _logger;

    public DiscountCodeGrpcService(
        ILogger<DiscountCodeGrpcService> logger,
        IDiscountCodeService discountCodeService)
    {
        _logger = logger;
        _discountCodeService = discountCodeService;
    }
    
    public override async Task<GenerateCodesResponse> GenerateCodes(GenerateCodesRequest request, ServerCallContext context)
    {
        var result = await _discountCodeService.GenerateCodesAsync(request.Count);
        
        return new GenerateCodesResponse
        {
            Success = result.IsSuccessful,
            Codes = { result.Codes },
            ErrorMessage = result.ErrorMessage
        };
    }
    
    public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
    {
        var result = await _discountCodeService.UseCodeAsync(request.Code);
        
        return new UseCodeResponse
        {
            Status = (UseCodeResponse.Types.Status)result.Status,
            Message = result.Message
        };
    }
}