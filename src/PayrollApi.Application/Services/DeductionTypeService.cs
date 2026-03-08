using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.DeductionType;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class DeductionTypeService : IDeductionTypeService
{
    private readonly IUnitOfWork _uow;

    public DeductionTypeService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<DeductionTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _uow.DeductionTypes.GetAllAsync(ct);
        return items.Select(ToDto);
    }

    public async Task<DeductionTypeDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _uow.DeductionTypes.GetByIdAsync(id, ct)
            ?? throw new AppException($"Deduction type {id} not found.");
        return ToDto(item);
    }

    public async Task<DeductionTypeDto> CreateAsync(CreateDeductionTypeDto dto, string createdBy, CancellationToken ct = default)
    {
        if (!await _uow.DeductionTypes.IsNameUniqueAsync(dto.Name.Trim(), cancellationToken: ct))
            throw new AppException($"A deduction type named '{dto.Name}' already exists.");

        var entity = new DeductionType
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsMandatory = dto.IsMandatory,
            DefaultAmount = dto.DefaultAmount,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
        };

        await _uow.DeductionTypes.AddAsync(entity, ct);
        await _uow.CommitAsync(ct);
        return ToDto(entity);
    }

    public async Task<DeductionTypeDto> UpdateAsync(int id, UpdateDeductionTypeDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.DeductionTypes.GetByIdAsync(id, ct)
            ?? throw new AppException($"Deduction type {id} not found.");

        if (!await _uow.DeductionTypes.IsNameUniqueAsync(dto.Name.Trim(), excludeId: id, cancellationToken: ct))
            throw new AppException($"A deduction type named '{dto.Name}' already exists.");

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
        entity.IsMandatory = dto.IsMandatory;
        entity.DefaultAmount = dto.DefaultAmount;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.Now;
        entity.UpdatedBy = updatedBy;

        await _uow.DeductionTypes.UpdateAsync(entity, ct);
        await _uow.CommitAsync(ct);
        return ToDto(entity);
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        _ = await _uow.DeductionTypes.GetByIdAsync(id, ct)
            ?? throw new AppException($"Deduction type {id} not found.");

        await _uow.DeductionTypes.DeleteAsync(id, deletedBy, ct);
        await _uow.CommitAsync(ct);
    }

    private static DeductionTypeDto ToDto(DeductionType e) => new(
        e.Id, e.Name, e.Description, e.IsMandatory, e.DefaultAmount, e.IsActive
    );
}
