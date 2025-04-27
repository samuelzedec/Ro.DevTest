using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands;

public class DeleteSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IValidator<DeleteSaleCommand>> _mockValidator;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<DeleteSaleCommandHandler>> _mockLogger;
    private readonly DeleteSaleCommandHandler _handler;

    public DeleteSaleCommandHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockValidator = new Mock<IValidator<DeleteSaleCommand>>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<DeleteSaleCommandHandler>>();

        _handler = new DeleteSaleCommandHandler(
            _mockSaleRepository.Object,
            _mockProductRepository.Object,
            _mockValidator.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should delete sale successfully when all inputs are correct")]
    public async Task Handle_WithValidInput_ShouldDeleteSaleSuccessfully()
    {
        // Arrange
        var existingSale = new Domain.Entities.Sale
        {
            Id = new Faker().Random.Guid(),
            CustomerId = new Faker().Random.Guid(),
            ProductId = new Faker().Random.Guid(),
            Quantity = new Faker().Random.Int(20, 40),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            EPaymentMethod = new Faker().PickRandom<EPaymentMethod>()
        };

        var existingProduct = new Domain.Entities.Product
        {
            Id = existingSale.ProductId,
            AvailableQuantity = new Faker().Random.Int(20, 100),
            Name = new Faker().Commerce.ProductName(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m)
        };

        var command = new DeleteSaleCommand(existingSale.Id);

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<DeleteSaleCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(existingSale.CustomerId.ToString());

        _mockSaleRepository
            .Setup(sr => sr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .ReturnsAsync(existingSale);

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync(existingProduct);

        _mockSaleRepository
            .Setup(sr => sr.UpdateAsync(It.IsAny<Domain.Entities.Sale>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockProductRepository
            .Setup(pr => pr.UpdateAsync(It.IsAny<Domain.Entities.Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Compra excluída com sucesso.");
    }
    
    [Fact(DisplayName = "Should return failure when sale is not found or belongs to another customer")]
    public async Task Handle_WithNonExistentSale_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteSaleCommand(new Faker().Random.Guid());
        
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<DeleteSaleCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _mockSaleRepository
            .Setup(sr => sr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .ReturnsAsync((Domain.Entities.Sale)null!);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Compra não encontrada.");
    }
    
    [Fact(DisplayName = "Should return failure when associated product is no longer available")]
    public async Task Handle_WithUnavailableProduct_ShouldReturnFailure()
    {
        // Arrange
        var existingSale = new Domain.Entities.Sale
        {
            Id = new Faker().Random.Guid(),
            CustomerId = new Faker().Random.Guid(),
            ProductId = new Faker().Random.Guid(),
            Quantity = new Faker().Random.Int(20, 40),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            EPaymentMethod = new Faker().PickRandom<EPaymentMethod>()
        };

        var command = new DeleteSaleCommand(existingSale.Id);

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<DeleteSaleCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(existingSale.CustomerId.ToString());

        _mockSaleRepository
            .Setup(sr => sr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .ReturnsAsync(existingSale);

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync((Domain.Entities.Product)null!);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Você não pode excluir a compra porque o produto não está mais disponível.");
    }
}