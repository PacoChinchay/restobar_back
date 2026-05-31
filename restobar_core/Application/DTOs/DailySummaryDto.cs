namespace restobar_core.Application.DTOs;

public class DailySummaryDto
{
    public decimal TotalAmount { get; set; }
    public int TotalSales { get; set; }
    public ByPaymentMethodDto ByPaymentMethod { get; set; } = new();
    public List<SaleDto> RecentSales { get; set; } = [];
}

public class ByPaymentMethodDto
{
    public decimal Efectivo { get; set; }
    public decimal Yape { get; set; }
    public decimal Plin { get; set; }
    public decimal Transferencia { get; set; }
}
