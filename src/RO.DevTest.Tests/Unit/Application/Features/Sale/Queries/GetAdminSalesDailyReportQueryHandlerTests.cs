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
using RO.DevTest.Application.Features.Sale.Queries.GetAdminSalesDailyReportQuery;
using RO.DevTest.Domain.ReadModels;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetAdminSalesDailyReportQueryHandlerTests
{
    private readonly Mock<IAdminSalesSummaryRepository> _mockAdminSalesSummaryRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<GetAdminSalesDailyReportQuery>> _mockValidator;
    private readonly Mock<ILogger<GetAdminSalesDailyReportQueryHandler>> _mockLogger;
    private readonly GetAdminSalesDailyReportQueryHandler _handler;

    public GetAdminSalesDailyReportQueryHandlerTests()
    {
        _mockAdminSalesSummaryRepository = new Mock<IAdminSalesSummaryRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<GetAdminSalesDailyReportQuery>>();
        _mockLogger = new Mock<ILogger<GetAdminSalesDailyReportQueryHandler>>();

        _handler = new GetAdminSalesDailyReportQueryHandler(
            _mockAdminSalesSummaryRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return daily sales report when request is valid")]
    public async Task Handle_ValidRequest_ReturnsDailySalesReport()
    {
        // Arrange
        var adminId = new Faker().Random.Guid();
        var adminIdString = adminId.ToString();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var query = new GetAdminSalesDailyReportQuery();

        var fakeSalesSummaries = new Faker<AdminSalesSummary>()
            .RuleFor(s => s.AdminId, f => adminId)
            .RuleFor(s => s.AdminUsername, f => f.Internet.UserName())
            .RuleFor(s => s.ProductId, f => f.Random.Guid())
            .RuleFor(s => s.ProductName, f => f.Commerce.ProductName())
            .RuleFor(s => s.TransactionDate, f => f.Date.Between(startDate, endDate))
            .RuleFor(s => s.TotalValue, f => f.Random.Decimal(100, 5000))
            .RuleFor(s => s.TransactionCount, f => f.Random.Int(1, 50))
            .RuleFor(s => s.TotalItemsSold, f => f.Random.Int(1, 100))
            .Generate(20);
            
        var mockDbSet = fakeSalesSummaries.AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetAdminSalesDailyReportQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(adminIdString);

        _mockAdminSalesSummaryRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<AdminSalesSummary, bool>>>(),
                It.IsAny<Expression<Func<AdminSalesSummary, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Vendas encontradas");
    }

    [Fact(DisplayName = "Should return unauthorized when user is not admin")]
    public async Task Handle_UserNotAdmin_ReturnsUnauthorized()
    {
        // Arrange
        var query = new GetAdminSalesDailyReportQuery();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetAdminSalesDailyReportQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Somente administradores têm acesso a essa informação.");
    }

    [Fact(DisplayName = "Should return validation error when request is invalid")]
    public async Task Handle_InvalidRequest_ReturnsValidationError()
    {
        // Arrange
        var query = new GetAdminSalesDailyReportQuery();

        List<ValidationFailure> validationFailures =
            [new ValidationFailure("PageNumber", "O número da página deve ser maior que zero")];

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetAdminSalesDailyReportQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("O número da página deve ser maior que zero");
    }
}