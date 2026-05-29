using restobar_core.Domain.Enums;

namespace restobar_core.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string PinHash { get; set; } = string.Empty;
    public decimal? MonthlySalary { get; set; }
}
