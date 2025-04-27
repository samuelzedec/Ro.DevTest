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
using RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands;

public class UpdateSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IValidator<UpdateSaleCommand>> _mockValidator;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<UpdateSaleCommandHandler>> _mockLogger;
    private readonly UpdateSaleCommandHandler _handler;

    public UpdateSaleCommandHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockValidator = new Mock<IValidator<UpdateSaleCommand>>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<UpdateSaleCommandHandler>>();

        _handler = new UpdateSaleCommandHandler(
            _mockSaleRepository.Object,
            _mockValidator.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should update sale payment method when all inputs are correct")]
    public async Task Handle_WithValidInput_ShouldUpdateSaleSuccessfully()
    {
        // Arrange
        var existingSale = new Domain.Entities.Sale
        {
            Id = new Faker().Random.Guid(),
            AdminId = new Faker().Random.Guid(),
            ProductId = new Faker().Random.Guid(),
            CustomerId =new Faker().Random.Guid(),
            EPaymentMethod = new Faker().PickRandom<EPaymentMethod>(),
            Quantity = new Faker().Random.Int(20, 100),
            UnitPrice = new Faker().Random.Decimal(9.99m, 599.99m)
        };

        var command = new UpdateSaleCommand(
            existingSale.Id,
            new Faker().PickRandom<EPaymentMethod>()
        );
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateSaleCommand>(),
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
        
        _mockSaleRepository
            .Setup(sr => sr.UpdateAsync(
                It.IsAny<Domain.Entities.Sale>(),
                It.IsAny<CancellationToken>()));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Compra atualizada");
    }
    
    [Fact(DisplayName = "Should return failure when sale does not exist or belongs to another customer")]
    public async Task Handle_WithNonExistentSale_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateSaleCommand(
            new Faker().Random.Guid(),
            new Faker().PickRandom<EPaymentMethod>()
        );
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateSaleCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());
        
        _mockSaleRepository
            .Setup(sr => sr.GetAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .ReturnsAsync((Domain.Entities.Sale)null!);
        
        _mockSaleRepository
            .Setup(sr => sr.UpdateAsync(
                It.IsAny<Domain.Entities.Sale>(),
                It.IsAny<CancellationToken>()));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Message.Should().ContainSingle().Which.Should().Be("Compra não encontrada");
    }
    
    [Fact(DisplayName = "Should return failure when validation fails")]
    public async Task Handle_WithInvalidInput_ShouldReturnValidationErrors()
    {
        // Arrange
        var command = new UpdateSaleCommand(
            new Faker().Random.Guid(),
            new Faker().PickRandom<EPaymentMethod>()
        );

        List<ValidationFailure> validationFailure = 
            [new("EPaymentMethod","Método de pagamento inválido")]; 
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateSaleCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailure));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Método de pagamento inválido");
    }
}