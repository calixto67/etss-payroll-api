using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Application.Mappings;
using PayrollApi.Application.Services;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;
using PayrollApi.Domain.Interfaces.Repositories;

namespace PayrollApi.UnitTests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<EmployeeService>> _loggerMock;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<EmployeeService>>();

        _unitOfWorkMock.Setup(u => u.Employees).Returns(_employeeRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeMappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new EmployeeService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployeeDto()
    {
        // Arrange
        var employee = BuildEmployee(id: 1, code: "EMP-001");
        _employeeRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(employee);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.EmployeeCode.Should().Be("EMP-001");
        result.FullName.Should().Be("Juan Dela Cruz");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Employee?)null);

        // Act
        var act = () => _sut.GetByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesAndReturnsEmployee()
    {
        // Arrange
        var dto = BuildCreateDto(code: "EMP-002", email: "juan@etss.com");

        _employeeRepoMock.Setup(r => r.IsEmployeeCodeUniqueAsync("EMP-002", null, default)).ReturnsAsync(true);
        _employeeRepoMock.Setup(r => r.IsEmailUniqueAsync("juan@etss.com", null, default)).ReturnsAsync(true);
        _employeeRepoMock.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .ReturnsAsync((Employee e, CancellationToken _) => { e.Id = 10; return e; });
        _unitOfWorkMock.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(dto, "admin");

        // Assert
        result.Should().NotBeNull();
        result.EmployeeCode.Should().Be("EMP-002");
        result.Email.Should().Be("juan@etss.com");
        _unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ThrowsConflictException()
    {
        // Arrange
        var dto = BuildCreateDto(code: "EMP-DUP");
        _employeeRepoMock.Setup(r => r.IsEmployeeCodeUniqueAsync("EMP-DUP", null, default)).ReturnsAsync(false);

        // Act
        var act = () => _sut.CreateAsync(dto, "admin");

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*EMP-DUP*");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ThrowsConflictException()
    {
        // Arrange
        var dto = BuildCreateDto(code: "EMP-003", email: "dup@etss.com");
        _employeeRepoMock.Setup(r => r.IsEmployeeCodeUniqueAsync("EMP-003", null, default)).ReturnsAsync(true);
        _employeeRepoMock.Setup(r => r.IsEmailUniqueAsync("dup@etss.com", null, default)).ReturnsAsync(false);

        // Act
        var act = () => _sut.CreateAsync(dto, "admin");

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*dup@etss.com*");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_SoftDeletes()
    {
        // Arrange
        var employee = BuildEmployee(id: 5, code: "EMP-005");
        _employeeRepoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync(employee);
        _employeeRepoMock.Setup(r => r.DeleteAsync(5, "admin", default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);

        // Act
        await _sut.DeleteAsync(5, "admin");

        // Assert
        _employeeRepoMock.Verify(r => r.DeleteAsync(5, "admin", default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _employeeRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Employee?)null);

        // Act
        var act = () => _sut.DeleteAsync(999, "admin");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static Employee BuildEmployee(int id, string code) => new()
    {
        Id = id,
        EmployeeCode = code,
        FirstName = "Juan",
        LastName = "Dela Cruz",
        Email = $"{code.ToLower()}@etss.com",
        DateOfBirth = new DateTime(1990, 1, 1),
        HireDate = new DateTime(2020, 1, 1),
        Status = EmploymentStatus.Active,
        EmploymentType = EmploymentType.FullTime,
        DepartmentId = 1,
        PositionId = 1,
        BasicSalary = 30000m,
        BankAccountNumber = "1234567890",
        BankName = "BDO",
        TaxIdentificationNumber = "123456789",
        SssNumber = "12-3456789-0",
        PhilHealthNumber = "12-345678901-2",
        PagIbigNumber = "1234-5678-9012",
    };

    private static CreateEmployeeDto BuildCreateDto(string code, string email = "test@etss.com") => new()
    {
        EmployeeCode = code,
        FirstName = "Juan",
        LastName = "Dela Cruz",
        Email = email,
        DateOfBirth = new DateTime(1990, 1, 1),
        HireDate = new DateTime(2020, 1, 1),
        DepartmentId = 1,
        PositionId = 1,
        BasicSalary = 30000m,
        BankAccountNumber = "1234567890",
        BankName = "BDO",
        TaxIdentificationNumber = "123456789",
        SssNumber = "12-3456789-0",
        PhilHealthNumber = "12-345678901-2",
        PagIbigNumber = "1234-5678-9012",
        EmploymentType = 1,
    };
}
