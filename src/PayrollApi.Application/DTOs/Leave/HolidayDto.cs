namespace PayrollApi.Application.DTOs.Leave;

public record HolidayDto(
    int    Id,
    string Name,
    string Date,
    string Type,
    string Region,
    bool   IsRecurring
);

public record CreateHolidayDto(
    string Name,
    string Date,
    string Type,
    string Region,
    bool   IsRecurring
);
