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
using RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetMyPurchasesQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<GetMyPurchasesQuery>> _mockValidator;
    private readonly Mock<ILogger<GetMyPurchasesQueryHandler>> _mockLogger;
    private readonly GetMyPurchasesQueryHandler _handler;

    public GetMyPurchasesQueryHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<GetMyPurchasesQuery>>();
        _mockLogger = new Mock<ILogger<GetMyPurchasesQueryHandler>>();

        _handler = new GetMyPurchasesQueryHandler(
            _mockSaleRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return customer purchases when request is valid")]
    public async Task Handle_ValidRequest_ReturnsCustomerPurchases()
    {
        // Arrange
        var customerId = new Faker().Random.Guid();
        var customerIdString = customerId.ToString();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var query = new GetMyPurchasesQuery();

        var fakeSales = new Faker<Domain.Entities.Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.CustomerId, f => customerId)
            .RuleFor(s => s.TransactionDate, f => f.Date.Between(startDate, endDate))
            .RuleFor(s => s.ProductId, f => f.Random.Guid())
            .RuleFor(s => s.Product, f => new Domain.Entities.Product 
            { 
                Id = f.Random.Guid(),
                Name = f.Commerce.ProductName(),
                Description = f.Commerce.ProductDescription(),
                UnitPrice = f.Random.Decimal(10, 200),
                AvailableQuantity = f.Random.Int(0, 100)
            })
            .RuleFor(s => s.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(s => s.DeletedAt, f => null)
            .Generate(20);
            
        var mockDbSet = fakeSales.AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetMyPurchasesQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(false);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(customerIdString);

        _mockSaleRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Compras encontradas");
    }

    [Fact(DisplayName = "Should return failure when user is admin")]
    public async Task Handle_UserIsAdmin_ReturnsFailure()
    {
        // Arrange
        var query = new GetMyPurchasesQuery();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetMyPurchasesQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockCurrentUserService
            .Setup(s => s.IsAdmin())
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Somente clientes têm compras");
    }

    [Fact(DisplayName = "Should return validation error when request is invalid")]
    public async Task Handle_InvalidRequest_ReturnsValidationError()
    {
        // Arrange
        var query = new GetMyPurchasesQuery()
        {
            PageNumber = 0,
            PageSize = 15
        };

        List<ValidationFailure> validationFailures =
            [new("PageNumber", "O número da página deve ser maior que zero")];

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetMyPurchasesQuery>(),
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