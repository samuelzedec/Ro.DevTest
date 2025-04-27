using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Queries.GetAllProductsQuery;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Queries;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IValidator<GetAllProductsQuery>> _mockValidator;
    private readonly Mock<ILogger<GetAllProductsQueryHandler>> _mockLogger;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockValidator = new Mock<IValidator<GetAllProductsQuery>>();
        _mockLogger = new Mock<ILogger<GetAllProductsQueryHandler>>();

        _handler = new GetAllProductsQueryHandler(
            _mockProductRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return paginated products when request is valid")]
    public async Task Handle_ValidRequest_ReturnsPaginatedProducts()
    {
        // Arrange
        var category = EProductCategory.Books;
        var adminId = new Faker().Random.Guid();
        var query = new GetAllProductsQuery(adminId, category);

        var admin = new Domain.Entities.Identity.User
        {
            Id = adminId,
            Name = new Faker().Person.FullName
        };

        var faker = new Faker<Domain.Entities.Product>()
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.AdminId, f => adminId)
            .RuleFor(p => p.Admin, f => admin)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(9.99m, 199.99m))
            .RuleFor(p => p.AvailableQuantity, f => f.Random.Int(10, 1000))
            .RuleFor(p => p.EProductCategory, f => category)
            .RuleFor(p => p.DeletedAt, f => null);

        var products = faker.Generate(20);
        var mockDbSet = products.AsQueryable().BuildMockDbSet();
        /* =========================================================================
         * .BuildMockDbSet(): Esta é uma extensão fornecida pela biblioteca MockQueryable.Moq que pega
         * o IQueryable e cria um mock completo de um DbSet.
         *
         * Configura o mock para suportar todas as operações de consulta do EF Core
         * como Where(), OrderBy(), Skip(), Take(), etc.
         *
         * Configura o suporte para operações assíncronas como ToListAsync(),
         * FirstOrDefaultAsync(), etc.
         * ========================================================================= */

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetAllProductsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProductRepository
            .Setup(pr => pr.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Produtos encontrados");
    }

    [Fact(DisplayName = "Should return validation error when request is invalid")]
    public async Task Handle_InvalidRequest_ReturnsValidationError()
    {
        // Arrange 
        var query = new GetAllProductsQuery(
            new Faker().Random.Guid(),
            EProductCategory.Books,
            0
        );

        List<ValidationFailure> validationFailture =
            [new ValidationFailure("PageNumber", "O número da página deve ser maior que zero")];

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetAllProductsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailture));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("O número da página deve ser maior que zero");
    }

    [Fact(DisplayName = "Should return database error when connection fails")]
    public async Task Handle_DatabaseConnectionFails_ReturnsInternalServerError()
    {
        // Arrange
        var query = new GetAllProductsQuery(
            new Faker().Random.Guid(),
            new Faker().PickRandom<EProductCategory>(),
            new Faker().Random.Int(1, 5),
            new Faker().Random.Int(15, 30)
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetAllProductsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _mockProductRepository
            .Setup(pr => pr.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Product, object>>[]>()))
            .Throws(new NpgsqlException("Connection refused. Check that the hostname and port are correct and that the postmaster is accepting TCP/IP connections"));
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().ContainSingle().Which.Should().Be("Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Connection refused. Check that the hostname and port are correct and that the postmaster is accepting TCP/IP connections")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}