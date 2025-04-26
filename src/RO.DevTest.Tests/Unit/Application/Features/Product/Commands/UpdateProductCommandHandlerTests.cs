using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<UpdateProductCommand>> _mockValidator;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _mockLogger;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<UpdateProductCommand>>();
        _mockLogger = new Mock<ILogger<UpdateProductCommandHandler>>();

        _handler = new UpdateProductCommandHandler(
            _mockProductRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Handle with valid update request should update product and return success result.")]
    public async Task Handle_WithValidUpdateRequest_UpdatesProductAndReturnsSuccess()
    {
        // Arrange
        var productGuid = new Faker().Random.Guid();
        var command = new UpdateProductCommand(
            productGuid,
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            new Faker().Random.Decimal(9.99m, 599.99m),
            new Faker().Random.Int(1, 100)
        );

        var existingProduct = new Domain.Entities.Product
        {
            Id = productGuid,
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            AvailableQuantity = new Faker().Random.Int(1, 100),
            ProductCategory = ProductCategory.Automotive
        };
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>()))
            .ReturnsAsync(existingProduct);
        
        _mockProductRepository
            .Setup(pr => pr.UpdateAsync(
                It.IsAny<Domain.Entities.Product>(),
                It.IsAny<CancellationToken>()));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto atualizado");
    }

    [Fact(DisplayName = "Handle when product not found should return failure with appropriate message.")]
    public async Task Handle_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateProductCommand(
            new Faker().Random.Guid(),
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            new Faker().Random.Decimal(9.99m, 599.99m),
            new Faker().Random.Int(1, 100)
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>()))
            .ReturnsAsync((Domain.Entities.Product)null!);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto não encontrado ou você não tem acesso");
    }

    [Fact(DisplayName = "Handle with invalid request should return validation errors.")]
    public async Task Handle_InvalidRequest_ReturnsValidationErrors()
    {
        var command = new UpdateProductCommand(
            new Faker().Random.Guid(),
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            0,
            new Faker().Random.Int(1, 100)
        );
        
        List<ValidationFailure> validationFailures = [new("UnitPrice", "Preço unitário deve ser maior que zero")];
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Preço unitário deve ser maior que zero");
    }
}