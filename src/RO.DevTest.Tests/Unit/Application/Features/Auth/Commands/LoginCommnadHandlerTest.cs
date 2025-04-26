using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.Auth.Commands;

public class LoginCommnadHandlerTest
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IValidator<LoginCommand>> _mockValidator;
    private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
    private readonly LoginCommandHandler _handler;

    public LoginCommnadHandlerTest()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockTokenService = new Mock<ITokenService>();
        _mockValidator = new Mock<IValidator<LoginCommand>>();
        _mockLogger = new Mock<ILogger<LoginCommandHandler>>();

        _handler = new LoginCommandHandler(
            _mockIdentityAbstractor.Object,
            _mockTokenService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Login with valid credentials should return success with a token and user information.")]
    public async Task Handle_LoginWithValidCredentials_ReturnsSuccessWithDisplayName()
    {
        // Arrange
        var userEmail = new Faker().Internet.Email();
        var command = new LoginCommand(
            userEmail,
            new Faker().Internet.Password()
        );
        var existingUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = userEmail
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _mockIdentityAbstractor
            .Setup(id => id.PasswordSignInAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockIdentityAbstractor
            .Setup(id => id.GetUserRolesAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(["Admin"]);

        _mockTokenService
            .Setup(ts => ts.GenerateAccessToken(It.IsAny<Domain.Entities.Identity.User>()))
            .Returns(new Faker().Random.String2(100));

        _mockTokenService
            .Setup(ts => ts.CreateRefreshTokenAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(new Faker().Random.Guid().ToString());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        result.Data?.Role.Should().Be("Admin");
        result.Message.Should().ContainSingle().Which.Should().Be("Login realizado com sucesso");
    }

    [Fact(DisplayName = "Login with invalid password should return failure with an appropriate message.")]
    public async Task Handle_LoginWithInvalidPassword_ReturnsFailureWithMessage()
    {
        // Arrange
        var userEmail = new Faker().Internet.Email();
        var command = new LoginCommand(
            userEmail,
            new Faker().Internet.Password()
        );
        var existingUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = userEmail
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _mockIdentityAbstractor
            .Setup(id => id.PasswordSignInAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Email/usuário ou senha inválidos");
    }

    [Fact(DisplayName = "Login with user not found should return 404 not found response.")]
    public async Task Handle_UserNotFound_ReturnsNotFoundFailure()
    {
        // Arrange
        var command = new LoginCommand(
            new Faker().Internet.Email(),
            new Faker().Internet.Password()
        );
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário não encontrado");
    }
}