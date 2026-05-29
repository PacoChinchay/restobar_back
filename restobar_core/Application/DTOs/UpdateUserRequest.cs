namespace restobar_core.Application.DTOs;

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Pin { get; set; }
    public decimal? MonthlySalary { get; set; }
}
