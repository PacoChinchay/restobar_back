namespace restobar_core.Domain.Entities;

public class CashSession
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime OpenedAt { get; set; }
    public string OpenedBy { get; set; } = "";
    public List<CashMovement> Movements { get; set; } = [];
}
