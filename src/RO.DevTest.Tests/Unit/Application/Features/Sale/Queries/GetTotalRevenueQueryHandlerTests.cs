using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;
using RO.DevTest.Domain.ReadModels;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetTotalRevenueQueryHandlerTests
{
    private readonly Mock<IAdminSalesSummaryRepository> _mockAdminSalesSummaryRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<GetTotalRevenueQuery>> _mockValidator;
    private readonly Mock<ILogger<GetTotalRevenueQueryHandler>> _mockLogger;
    private readonly GetTotalRevenueQueryHandler _handler;

    public GetTotalRevenueQueryHandlerTests()
    {
        _mockAdminSalesSummaryRepository = new Mock<IAdminSalesSummaryRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<GetTotalRevenueQuery>>();
        _mockLogger = new Mock<ILogger<GetTotalRevenueQueryHandler>>();

        _handler = new GetTotalRevenueQueryHandler(
            _mockAdminSalesSummaryRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return total revenue successfully")]
    public async Task Handle_ValidRequest_ShouldReturnTotalRevenueSuccessfully()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var query = new GetTotalRevenueQuery
        {
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        var fakeSalesSummaries = new Faker<AdminSalesSummary>()
            .RuleFor(s => s.AdminId, adminId)
            .RuleFor(s => s.AdminUsername, f => f.Internet.UserName())
            .RuleFor(s => s.ProductId, f => f.Random.Guid())
            .RuleFor(s => s.ProductName, f => f.Commerce.ProductName())
            .RuleFor(s => s.TransactionDate, f => f.Date.Recent(30))
            .RuleFor(s => s.TotalValue, f => f.Random.Decimal(100, 5000))
            .RuleFor(s => s.TransactionCount, f => f.Random.Int(1, 50))
            .RuleFor(s => s.TotalItemsSold, f => f.Random.Int(1, 100))
            .Generate(20);

        var mockTotalSummaryDbSet = fakeSalesSummaries.AsQueryable().BuildMockDbSet();
        var mockTopProductsDbSet = fakeSalesSummaries.AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetTotalRevenueQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(adminId.ToString());

        _mockAdminSalesSummaryRepository
            .SetupSequence(r => r.GetQueryable(
                It.IsAny<Expression<Func<AdminSalesSummary, bool>>>()))
            .Returns(mockTotalSummaryDbSet.Object)
            .Returns(mockTopProductsDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Data?.TotalValue.Should().BeGreaterThan(0);
        result.Data?.TopProducts.Should().NotBeEmpty().And.HaveCount(5);
        result.Message.Should().ContainSingle().Which.Should().Be("Faturamento total de todos os produtos durante esse período");
    }

    [Fact(DisplayName = "Should return failure when the user is not an administrator")]
    public async Task Handle_NonAdminUser_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetTotalRevenueQuery
        {
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetTotalRevenueQuery>(), 
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
        result.Message.Should().ContainSingle().Which.Should().Be("Somente administradores têm acesso a essa informação.");
    }

    [Fact(DisplayName = "Should return failure when no sales are found in the period")]
    public async Task Handle_NoSalesFound_ShouldReturnFailure()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var query = new GetTotalRevenueQuery
        {
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        var mockEmptyDbSet = new List<AdminSalesSummary>().AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetTotalRevenueQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(adminId.ToString());

        _mockAdminSalesSummaryRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<AdminSalesSummary, bool>>>()))
            .Returns(mockEmptyDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Não foram encontradas vendas para neste período especificado");
    }
}