namespace restobar_core.Application.DTOs;

public class PayOrderRequest
{
    public List<PaymentEntry> Payments { get; set; } = [];
    public string RegisteredBy { get; set; } = string.Empty;
}

public record PaymentEntry(string Method, decimal Amount);
