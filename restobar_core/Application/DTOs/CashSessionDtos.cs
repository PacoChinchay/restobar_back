namespace restobar_core.Application.DTOs;

public record CashMovementDto(
    int Id,
    string MovementType,
    decimal Amount,
    string Description,
    DateTime CreatedAt,
    string CreatedBy);

public record CashSessionDto(
    int Id,
    DateOnly Date,
    decimal InitialAmount,
    string OpenedBy,
    DateTime OpenedAt,
    List<CashMovementDto> Movements,
    decimal Balance);

public record OpenCashSessionRequest(decimal InitialAmount, string OpenedBy);

public record AddCashMovementRequest(
    string MovementType,
    decimal Amount,
    string Description,
    string CreatedBy);
