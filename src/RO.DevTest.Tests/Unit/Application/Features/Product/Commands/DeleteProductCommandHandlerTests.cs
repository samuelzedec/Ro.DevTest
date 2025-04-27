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
using RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<DeleteProductCommand>> _mockValidator;
    private readonly Mock<ILogger<DeleteProductCommandHandler>> _mockLogger;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<DeleteProductCommand>>();
        _mockLogger = new Mock<ILogger<DeleteProductCommandHandler>>();

        _handler = new DeleteProductCommandHandler(
            _mockProductRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Handle with valid delete request should mark product as deleted and return success.")]
    public async Task Handle_WithValidDeleteRequest_MarksProductAsDeletedAndReturnsSuccess()
    {
        var productGuid = new Faker().Random.Guid();
        var command = new DeleteProductCommand(productGuid);
        var existingProduct = new Domain.Entities.Product
        {
            Id = productGuid,
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            AvailableQuantity = new Faker().Random.Int(1, 100),
            EProductCategory = EProductCategory.Automotive
        };
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<DeleteProductCommand>(),
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
        result.Message.Should().ContainSingle().Which.Should().Be("Produto deletado com sucesso");
    }

    [Fact(DisplayName = "Handle when product not found should return failure with 404 status code.")]
    public async Task Handle_ProductNotFound_ReturnsNotFoundFailure()
    {
        // Arrange
        var command = new DeleteProductCommand(new Faker().Random.Guid());
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<DeleteProductCommand>(),
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
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto não encontrado");
    }

    [Fact(DisplayName = "Handle with invalid product id should return validation errors.")]
    public async Task Handle_InvalidProductId_ReturnsValidationErrors()
    {
        // Arrange
        var command = new DeleteProductCommand(new Faker().Random.Guid());

        List<ValidationFailure> validationFailures = 
            [new("ProductId", "O Id do produto é obrigatório")];
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<DeleteProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("O Id do produto é obrigatório");
    }
}