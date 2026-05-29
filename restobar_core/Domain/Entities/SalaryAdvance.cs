namespace restobar_core.Domain.Entities;

public class SalaryAdvance
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public string RegisteredBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
