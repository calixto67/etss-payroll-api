using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Services;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.UnitTests.Services;

/// <summary>
/// EmployeeService now uses ISqlExecutor with internal Row types.
/// These tests verify constructor wiring; detailed integration tests
/// should exercise the actual stored procedures.
/// </summary>
public class EmployeeServiceTests
{
    private readonly Mock<ISqlExecutor> _sqlMock;
    private readonly Mock<ILogger<EmployeeService>> _loggerMock;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sqlMock = new Mock<ISqlExecutor>();
        _loggerMock = new Mock<ILogger<EmployeeService>>();
        _sut = new EmployeeService(_sqlMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_CreatesInstance()
    {
        _sut.Should().NotBeNull();
    }
}
