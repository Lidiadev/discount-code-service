namespace DiscountCode.Application.Dtos;

public enum UseCodeStatus
{
    Success,
    NotFound,
    AlreadyUsed,
    ConcurrencyError
}