namespace DiscountCode.Application.Dtos;

public class UseCodeResult
{
    public UseCodeStatus Status { get; }
    public string Message { get; }
    
    public bool IsSuccessful { get; }
    
    private UseCodeResult(UseCodeStatus status, string message)
    {
        Status = status;
        Message = message;
        IsSuccessful = true;
    }
    
    private UseCodeResult(bool isSuccessful, UseCodeStatus status, string errorMessage)
    {
        Status = status;
        Message = errorMessage;
        IsSuccessful = isSuccessful;
    }

    public static UseCodeResult Success(UseCodeStatus status, string message) =>
        new(status, message);

    public static UseCodeResult Failure(UseCodeStatus status, string errorMessage) =>
        new(false, status, errorMessage);
}