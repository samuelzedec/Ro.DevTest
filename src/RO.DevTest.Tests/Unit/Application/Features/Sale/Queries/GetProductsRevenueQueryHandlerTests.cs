using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries.GetProductsRevenueQuery;
using RO.DevTest.Domain.ReadModels;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetProductsRevenueQueryHandlerTests
{
    private readonly Mock<IAdminSalesSummaryRepository> _mockAdminSalesSummaryRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<GetProductsRevenueQuery>> _mockValidator;
    private readonly Mock<ILogger<GetProductsRevenueQueryHandler>> _mockLogger;
    private readonly GetProductsRevenueQueryHandler _handler;

    public GetProductsRevenueQueryHandlerTests()
    {
        _mockAdminSalesSummaryRepository = new Mock<IAdminSalesSummaryRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<GetProductsRevenueQuery>>();
        _mockLogger = new Mock<ILogger<GetProductsRevenueQueryHandler>>();

        _handler = new GetProductsRevenueQueryHandler(
            _mockAdminSalesSummaryRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return products revenue when request is valid")]
    public async Task Handle_ValidRequest_ReturnsProductsRevenue()
    {
        // Arrange
        var adminId = new Faker().Random.Guid();
        var adminIdString = adminId.ToString();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var query = new GetProductsRevenueQuery();

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
                It.IsAny<GetProductsRevenueQuery>(),
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
                It.IsAny<Expression<Func<AdminSalesSummary, bool>>>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Relatório da venda dos produtos");
    }

    [Fact(DisplayName = "Should return unauthorized when user is not admin")]
    public async Task Handle_UserNotAdmin_ReturnsUnauthorized()
    {
        // Arrange
        var query = new GetProductsRevenueQuery();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetProductsRevenueQuery>(),
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
    
    [Fact(DisplayName = "An exception should be thrown when the sales view does not exist in the database")]
    public async Task Handle_WhenViewNotExists_ShouldThrowException()
    {
        // Arrange
        var query = new GetProductsRevenueQuery();
        
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetProductsRevenueQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);
        
        _mockAdminSalesSummaryRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<AdminSalesSummary, bool>>>()))
            .Throws(new PostgresException(
                "ERROR",               
                "ERROR",                
                "42P01",                  
                "relation \"vw_admin_product_monthly_sales\" does not exist" 
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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("relation \"vw_admin_product_monthly_sales\" does not exist")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}