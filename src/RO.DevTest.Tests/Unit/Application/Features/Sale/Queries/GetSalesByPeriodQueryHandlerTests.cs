using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries.GetSalesByPeriodQuery;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetSalesByPeriodQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<GetSalesByPeriodQuery>> _mockValidator;
    private readonly Mock<ILogger<GetSalesByPeriodQueryHandler>> _mockLogger;
    private readonly GetSalesByPeriodQueryHandler _handler;

    public GetSalesByPeriodQueryHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<GetSalesByPeriodQuery>>();
        _mockLogger = new Mock<ILogger<GetSalesByPeriodQueryHandler>>();

        _handler = new GetSalesByPeriodQueryHandler(
            _mockSaleRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return sales successfully for an administrator")]
    public async Task Handle_ValidRequest_ShouldReturnSalesSuccessfully()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var query = new GetSalesByPeriodQuery
        {
            PageNumber = 1,
            PageSize = 10,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        var fakeSales = new Faker<Domain.Entities.Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.AdminId, adminId)
            .RuleFor(s => s.DeletedAt, (DateTime?)null)
            .RuleFor(s => s.TransactionDate, f => f.Date.Recent(30))
            .RuleFor(s => s.Product, new Faker<Domain.Entities.Product>().Generate())
            .RuleFor(s => s.Admin, new Faker<Domain.Entities.Identity.User>().Generate())
            .RuleFor(s => s.Customer, new Faker<Domain.Entities.Identity.User>().Generate())
            .Generate(15);

        var mockDbSet = fakeSales.AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSalesByPeriodQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(adminId.ToString());

        _mockSaleRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Message.Should().ContainSingle().Which.Should().Be("Vendas encontradas");
    }

    [Fact(DisplayName = "Should return failure when the user is not an administrator")]
    public async Task Handle_NonAdminUser_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetSalesByPeriodQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSalesByPeriodQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Somente administradores têm acesso");
    }

    [Fact(DisplayName = "Should return failure in case of a database connection error")]
    public async Task Handle_DatabaseConnectionError_ShouldReturnFailure()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var query = new GetSalesByPeriodQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSalesByPeriodQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(adminId.ToString());

        _mockSaleRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .Throws(new PostgresException(
                "ERROR", 
                "ERROR", 
                "08003", 
                "Database connection failure"
            ));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database connection failure")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}