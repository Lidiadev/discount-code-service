namespace DiscountCode.Application.Constants;

public static class ErrorMessage
{
    public const string CodeInvalid = "Code is invalid.";
    public const string CodeAlreadyUsed = "Code has already been used.";
    public const string CodeNotFound = "Discount code not found.";
    public const string ConcurrencyError = "Concurrency error occurred. Please try again.";
    public const string GenerationFailed = "Failed to generate codes.";
    public const string MaximumCountExceeded = "Requested code count exceeds maximum allowed.";
    public const string InvalidCount = "Requested code count is not valid.";
    public const string ErrorNoCodeGenerated = "Failed to generate codes.";
    public const string ErrorGeneratingCodes = "An error occurred while generating codes.";
}