using DiscountCode.Domain.Entities;

namespace DiscountCode.Domain;

public interface IDiscountCodeRepository
{
    Task<IList<AvailableDiscountCode>> GetAvailableCodesAsync(int count);
    Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes);
    Task<bool> MoveToDiscountCodesAsync(IList<string> codes);
    Task<Entities.DiscountCode> GetDiscountCodeAsync(string code);
    Task<bool> MarkAsModifiedAsync(Entities.DiscountCode discountCode);
}