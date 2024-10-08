using DiscountCode.Application.Dtos;

namespace DiscountCode.Application;

public interface IDiscountCodeService
{
    Task<GenerateCodesResponse> GenerateCodesAsync(int count);
    Task<UseCodeResult> UseCodeAsync(string code);
}