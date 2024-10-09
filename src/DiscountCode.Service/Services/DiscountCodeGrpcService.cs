using DiscountCode.Application;
using DiscountCode.Application.Dtos;
using DiscountService;
using Grpc.Core;
using GenerateCodesResponse = DiscountService.GenerateCodesResponse;
using UseCodeRequest = DiscountService.UseCodeRequest;

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
        var result = await _discountCodeService.GenerateCodesAsync(
            new GenerateCodeRequest
            {
                Count = request.Count,
                Length = request.Length,
            });
        
        return new GenerateCodesResponse
        {
            Success = result.IsSuccessful,
            Codes = { result.Codes },
            ErrorMessage = result.ErrorMessage
        };
    }
    
    public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
    {
        var result =
            await _discountCodeService.UseCodeAsync(new Application.Dtos.UseCodeRequest { Code = request.Code });
        
        return new UseCodeResponse
        {
            Status = (UseCodeResponse.Types.Status)result.Status,
            Message = result.Message
        };
    }
}