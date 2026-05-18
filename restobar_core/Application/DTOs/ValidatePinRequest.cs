namespace restobar_core.Application.DTOs;

public class ValidatePinRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}
