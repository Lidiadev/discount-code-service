using DiscountCode.Domain.Entities;

namespace DiscountCode.Domain;

public interface IDiscountCodeRepository
{
    Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes);
    Task<IList<AvailableDiscountCode>> GenerateCodesAsync(int count);
    Task<bool> MoveToDiscountCodesAsync(IList<string> codes);
    Task<Entities.DiscountCode> GetDiscountCodeAsync(string code);
    Task<bool> MarkAsModifiedAsync(Entities.DiscountCode discountCode);
}