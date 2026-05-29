namespace restobar_core.Application.DTOs;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public decimal? MonthlySalary { get; set; }
}
