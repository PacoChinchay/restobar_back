namespace restobar_core.Application.DTOs;

public class PayOrderRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string RegisteredBy { get; set; } = string.Empty;
}
