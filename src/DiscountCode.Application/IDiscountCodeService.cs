using DiscountCode.Application.Dtos;
using DiscountCode.Domain.Entities;

namespace DiscountCode.Application;

public interface IDiscountCodeService
{
    Task<GenerateCodesResponse> GenerateCodesAsync(int count);
    Task<UseCodeResult> UseCodeAsync(string code);
    Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes);
}