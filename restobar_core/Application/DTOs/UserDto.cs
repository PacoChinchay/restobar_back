namespace restobar_core.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal? MonthlySalary { get; set; }
}
