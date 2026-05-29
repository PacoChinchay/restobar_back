namespace restobar_core.Domain.Entities;

public class CashMovement
{
    public int Id { get; set; }
    public int CashSessionId { get; set; }
    public CashSession Session { get; set; } = null!;
    public string MovementType { get; set; } = ""; // "ingreso" | "egreso"
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
}
