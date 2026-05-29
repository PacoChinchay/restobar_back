namespace restobar_core.Application.DTOs;

public record SalaryAdvanceDto(
    int Id,
    string UserId,
    string EmployeeName,
    decimal Amount,
    DateOnly Date,
    string? Notes,
    string RegisteredBy,
    DateTime CreatedAt);

public record EmployeeAdvanceSummaryDto(
    string UserId,
    string Name,
    string Role,
    decimal MonthlySalary,
    decimal WeeklySalary,
    decimal AdvancedThisWeek,
    decimal Remaining,
    List<SalaryAdvanceDto> WeekAdvances);

public record CreateAdvanceRequest(
    string UserId,
    string EmployeeName,
    decimal Amount,
    string? Notes,
    string RegisteredBy);
