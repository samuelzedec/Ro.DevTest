using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands;

/* ========================================
 * Padrão de nomeação de testes unitários
 * [Método]_[Cenário]_[ResultadoEsperado]
 * ======================================== */

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<CreateProductCommand>> _mockValidator;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _mockLogger;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<CreateProductCommand>>();
        _mockLogger = new Mock<ILogger<CreateProductCommandHandler>>();

        _handler = new CreateProductCommandHandler(
            _mockProductRepository.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Handle with valid product request and admin user should create product and return success result.")]
    public async Task Handle_WithValidProductRequestAndAdminUser_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateProductCommand(
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            new Faker().Random.Decimal(9.99m, 599.99m),
            new Faker().Random.Int(1, 100),
            ProductCategory.Automotive
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());

        _mockProductRepository
            .Setup(pr => pr.CreateAsync(
                It.IsAny<Domain.Entities.Product>(),
                It.IsAny<CancellationToken>()));
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Message.Should().ContainSingle().Which.Should().Be("Produto criado com sucesso");
    }

    [Fact(DisplayName = "Handle when non-admin tries to create product should return unauthorized error message.")]
    public async Task Handle_NonAdminCreatesProduct_ReturnsUnauthorizedError()
    {
        // Arrange
        var command = new CreateProductCommand(
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            new Faker().Random.Decimal(9.99m, 599.99m),
            new Faker().Random.Int(1, 100),
            ProductCategory.Automotive
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(false);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        result.Message.Should().ContainSingle().Which.Should().Be("Apenas administradores podem criar produtos");
    }

    [Fact(DisplayName = "Handle when database throws exception should return failure with internal server error.")]
    public async Task Handle_DatabaseThrowsException_ReturnsInternalServerErrorFailure()
    {
        // Arrange
        var command = new CreateProductCommand(
            new Faker().Commerce.ProductName(),
            new Faker().Commerce.ProductDescription(),
            new Faker().Random.Decimal(9.99m, 599.99m),
            new Faker().Random.Int(1, 100),
            ProductCategory.Automotive
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.IsAdmin())
            .Returns(true);

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());

        _mockProductRepository
            .Setup(pr => pr.CreateAsync(
                It.IsAny<Domain.Entities.Product>(),
                It.IsAny<CancellationToken>()))
            .Throws(new DbUpdateException("Error inserting product in the database"));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().ContainSingle().Which.Should().Be("Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inserting product in the database")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}