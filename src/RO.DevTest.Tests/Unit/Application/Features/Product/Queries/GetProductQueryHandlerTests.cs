using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Queries.GetProductQuery;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Queries;

public class GetProductQueryHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IValidator<GetProductQuery>> _mockValidator;
    private readonly Mock<ILogger<GetProductQueryHandler>> _mockLogger;
    private readonly GetProductQueryHandler _handler;

    public GetProductQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockValidator = new Mock<IValidator<GetProductQuery>>();
        _mockLogger = new Mock<ILogger<GetProductQueryHandler>>();

        _handler = new GetProductQueryHandler(
            _mockProductRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }
    
    [Fact(DisplayName = "Should return product details when product exists")]
    public async Task Handle_ExistingProduct_ReturnsProductDetails()
    {
        // Arrange
        var adminId = new Faker().Random.Guid();
        var query = new GetProductQuery(adminId);
        var admin = new Domain.Entities.Identity.User
        {
            Id = adminId,
            Name = new Faker().Person.FullName
        };

        var productFaker = new Domain.Entities.Product
        {
            Id = new Faker().Random.Guid(),
            AdminId = adminId,
            Admin = admin,
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 199.99m),
            AvailableQuantity = new Faker().Random.Int(10, 1000),
            EProductCategory = new Faker().PickRandom<EProductCategory>(),
            DeletedAt = null
        };
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetProductQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync(productFaker);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto encontrado");
    }
    
    [Fact(DisplayName = "Should return not found when product does not exist")]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var query = new GetProductQuery(new Faker().Random.Guid());
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetProductQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync((Domain.Entities.Product)null!);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto não encontrado");
    }
    
    [Fact(DisplayName = "Should return validation error when request is invalid")]
    public async Task Handle_InvalidRequest_ReturnsValidationError()
    {
        var query = new GetProductQuery(new Faker().Random.Guid());

        List<ValidationFailure> validationFailure = 
            [new ValidationFailure("ProductId", "O Id do produto não pode ser vazio")];
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetProductQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailure));
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("O Id do produto não pode ser vazio");
    }
}