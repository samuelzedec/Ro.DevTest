using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmailQuery;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Queries;

public class GetUserByNameOrEmailQueryHandlerTests
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<IValidator<GetUserByNameOrEmailQuery>> _mockValidator;
    private readonly Mock<ILogger<GetUserByNameOrEmailQueryHandler>> _mockLogger;
    private readonly GetUserByNameOrEmailQueryHandler _handler;

    public GetUserByNameOrEmailQueryHandlerTests()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockValidator = new Mock<IValidator<GetUserByNameOrEmailQuery>>();
        _mockLogger = new Mock<ILogger<GetUserByNameOrEmailQueryHandler>>();

        _handler = new GetUserByNameOrEmailQueryHandler(
            _mockIdentityAbstractor.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Handle should successfully retrieve user details with roles when user exists")]
    public async Task Handle_WhenUserExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var emailFaker = new Faker().Internet.Email();
        var query = new GetUserByNameOrEmailQuery(emailFaker);
        var existingCurrentUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = emailFaker,
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetUserByNameOrEmailQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingCurrentUser);

        _mockIdentityAbstractor
            .Setup(id => id.GetUserRolesAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(["Admin"]);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário encontrado");
    }

    [Fact(DisplayName = "Handle should return not found status with appropriate error message when user does not exist")]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var emailFaker = new Faker().Internet.Email();
        var query = new GetUserByNameOrEmailQuery(emailFaker);
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetUserByNameOrEmailQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário não encontrado");
        
    }
    
    [Fact(DisplayName = "Handle should return validation failure with detailed errors when request does not meet validation requirements")]
    public async Task Handle_WhenValidationFails_ShouldReturnBadRequestWithErrors()
    {
        // Arrange
        var query = new GetUserByNameOrEmailQuery(string.Empty);
        
        List<ValidationFailure> validationFailures = 
            [new("NameOrEmail", "O campo Nome/Email é obrigatório")];

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<GetUserByNameOrEmailQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("O campo Nome/Email é obrigatório");
    }
}