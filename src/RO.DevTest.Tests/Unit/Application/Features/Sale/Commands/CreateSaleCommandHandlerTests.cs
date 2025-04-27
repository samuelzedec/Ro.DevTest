using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands;

public class CreateSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IValidator<CreateSaleCommand>> _mockValidator;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<CreateSaleCommandHandler>> _mockLogger;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockValidator = new Mock<IValidator<CreateSaleCommand>>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<CreateSaleCommandHandler>>();

        _handler = new CreateSaleCommandHandler(
            _mockSaleRepository.Object,
            _mockProductRepository.Object,
            _mockValidator.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should create a valid sale when all inputs are correct")]
    public async Task Handle_WithValidInput_ShouldCreateSaleSuccessfully()
    {
        // Arrange
        var admin = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            Name = new Faker().Name.FullName()
        };

        var customer = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            Name = new Faker().Name.FullName()
        };

        var existingProduct = new Domain.Entities.Product
        {
            Id = new Faker().Random.Guid(),
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            AvailableQuantity = new Faker().Random.Int(20, 100),
            EProductCategory = EProductCategory.Automotive,
            AdminId = admin.Id
        };

        var command = new CreateSaleCommand(
            existingProduct.Id,
            new Faker().PickRandom<EPaymentMethod>(),
            new Faker().Random.Int(2, 15)
        );

        var newSale = new Domain.Entities.Sale
        {
            Id = new Faker().Random.Guid(),
            AdminId = admin.Id,
            Admin = admin,
            ProductId = existingProduct.Id,
            Product = existingProduct,
            CustomerId = customer.Id,
            Customer = customer,
            EPaymentMethod = command.EPaymentMethod,
            Quantity = command.Quantity,
            UnitPrice = existingProduct.UnitPrice
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateSaleCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(false);
        
        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(customer.Id.ToString());

        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync(existingProduct);

        _mockSaleRepository
            .Setup(sr => sr.CreateAsync(
                It.IsAny<Domain.Entities.Sale>(),
                It.IsAny<CancellationToken>()));

        _mockSaleRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .ReturnsAsync(newSale);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Message.Should().ContainSingle().Which.Should().Be("Compra realizada com sucesso");
    }
    
    [Fact(DisplayName = "Should return failure when product is out of stock")]
    public async Task Handle_WithProductOutOfStock_ShouldReturnFailure()
    {
        // Arrange
        var existingProduct = new Domain.Entities.Product
        {
            Id = new Faker().Random.Guid(),
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            AvailableQuantity = 0,
            EProductCategory = EProductCategory.Automotive,
            AdminId = new Faker().Random.Guid()
        };

        var command = new CreateSaleCommand(
            existingProduct.Id,
            new Faker().PickRandom<EPaymentMethod>(),
            new Faker().Random.Int(11, 15)
        );
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateSaleCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(false);
        
        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync(existingProduct);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto fora de estoque");
    }

    [Fact(DisplayName = "Should return failure when product quantity is insufficient")]
    public async Task Handle_WithInsufficientProductQuantity_ShouldReturnFailure()
    {
        // Arrange
        var existingProduct = new Domain.Entities.Product
        {
            Id = new Faker().Random.Guid(),
            Name = new Faker().Commerce.ProductName(),
            Description = new Faker().Commerce.ProductDescription(),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m),
            AvailableQuantity = new Faker().Random.Int(5, 10),
            EProductCategory = EProductCategory.Automotive,
            AdminId = new Faker().Random.Guid()
        };

        var command = new CreateSaleCommand(
            existingProduct.Id,
            new Faker().PickRandom<EPaymentMethod>(),
            new Faker().Random.Int(16, 20)
        );
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateSaleCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(false);
        
        _mockProductRepository
            .Setup(pr => pr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .ReturnsAsync(existingProduct);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Quantidade insuficiente disponível");
    }
}