namespace PayrollApi.Application.DTOs.DeductionType;

public record DeductionTypeDto(
    int Id, string Name, string? Description,
    bool IsMandatory, decimal DefaultAmount, bool IsActive
);

public record CreateDeductionTypeDto(
    string Name, string? Description,
    bool IsMandatory, decimal DefaultAmount, bool IsActive
);

public record UpdateDeductionTypeDto(
    string Name, string? Description,
    bool IsMandatory, decimal DefaultAmount, bool IsActive
);
