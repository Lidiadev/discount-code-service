using DiscountCode.Application.Dtos;
using DiscountCode.Domain.Entities;

namespace DiscountCode.Application;

public interface IDiscountCodeService
{
    Task<GenerateCodesResponse> GenerateCodesAsync(GenerateCodeRequest request);
    Task<UseCodeResult> UseCodeAsync(UseCodeRequest request);
    Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes);
}