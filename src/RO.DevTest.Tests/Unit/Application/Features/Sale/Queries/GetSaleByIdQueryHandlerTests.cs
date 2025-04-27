using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetSaleByIdQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IValidator<GetSaleByIdQuery>> _mockValidator;
    private readonly Mock<ILogger<GetSaleByIdQueryHandler>> _mockLogger;
    private readonly GetSaleByIdQueryHandler _handler;

    public GetSaleByIdQueryHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockValidator = new Mock<IValidator<GetSaleByIdQuery>>();
        _mockLogger = new Mock<ILogger<GetSaleByIdQueryHandler>>();

        _handler = new GetSaleByIdQueryHandler(
            _mockSaleRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return the sale successfully when the ID exists")]
    public async Task Handle_ExistingSale_ShouldReturnSaleSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        var fakeSale = new Faker<Domain.Entities.Sale>()
            .RuleFor(s => s.Id, saleId)
            .RuleFor(s => s.DeletedAt, (DateTime?)null)
            .RuleFor(s => s.Product, new Faker<Domain.Entities.Product>().Generate())
            .RuleFor(s => s.Customer, new Faker<Domain.Entities.Identity.User>().Generate())
            .RuleFor(s => s.Admin, new Faker<Domain.Entities.Identity.User>().Generate())
            .Generate();

        var mockDbSet = new List<Domain.Entities.Sale> { fakeSale }.AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSaleByIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockSaleRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Venda encontrada");
    }

    [Fact(DisplayName = "Should return failure when the sale does not exist")]
    public async Task Handle_NonExistingSale_ShouldReturnFailure()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        var mockDbSet = new List<Domain.Entities.Sale>().AsQueryable().BuildMockDbSet();

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSaleByIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockSaleRepository
            .Setup(r => r.GetQueryable(
                It.IsAny<Expression<Func<Domain.Entities.Sale, bool>>>(),
                It.IsAny<Expression<Func<Domain.Entities.Sale, object>>[]>()))
            .Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Venda não encontrada");
    }

    [Fact(DisplayName = "Should return failure when validation fails")]
    public async Task Handle_ValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        List<ValidationFailure> validationFailures = 
            [new ValidationFailure(nameof(GetSaleByIdQuery.SaleId), "O ID da venda é inválido")];

        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetSaleByIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("O ID da venda é inválido");
    }
}