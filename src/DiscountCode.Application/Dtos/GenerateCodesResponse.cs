namespace DiscountCode.Application.Dtos;

public class GenerateCodesResponse
{
    public bool IsSuccessful { get; }
    public IList<string> Codes { get; } 
    public string ErrorMessage { get; }
    
    private GenerateCodesResponse(IList<string> codes)
    {
        Codes = codes;
        IsSuccessful = true;
        ErrorMessage = string.Empty;
    }
    
    private GenerateCodesResponse(bool isSuccessful, string errorMessage)
    {
        Codes = new List<string>();
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
    }

    public static GenerateCodesResponse Success(IList<string> codes) =>
        new(codes);

    public static GenerateCodesResponse Failure(string errorMessage) =>
        new(false, errorMessage);
}