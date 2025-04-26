using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IValidator<RefreshTokenCommand>> _mockValidator;
    private readonly Mock<ILogger<RefreshTokenCommandHandler>> _mockLogger;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockTokenService = new Mock<ITokenService>();
        _mockValidator = new Mock<IValidator<RefreshTokenCommand>>();
        _mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();

        _handler = new RefreshTokenCommandHandler(
            _mockIdentityAbstractor.Object,
            _mockTokenService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Refresh token with valid data should return success with new tokens.")]
    public async Task Handle_ValidRefreshToken_ReturnsSuccessWithNewTokens()
    {
        // Arrange
        var currentGuid = new Faker().Random.Guid();
        var command = new RefreshTokenCommand(
            currentGuid,
            new Faker().Random.Guid().ToString()
        );
        var existingUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email()
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<RefreshTokenCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _mockTokenService
            .Setup(ts => ts.ValidationRefreshTokenAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

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
        result.Message.Should().ContainSingle().Which.Should().Be("Novos tokens de autenticação.");
    }

    [Fact(DisplayName = "Refresh token with invalid data should return failure response.")]
    public async Task Handle_InvalidRefreshToken_ReturnsFailure()
    {
        // Arrange
        var currentGuid = new Faker().Random.Guid();
        var command = new RefreshTokenCommand(
            currentGuid,
            new Faker().Random.Guid().ToString()
        );
        var existingUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email()
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<RefreshTokenCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _mockTokenService
            .Setup(ts => ts.ValidationRefreshTokenAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Informações inválidas.");
    }

    [Fact(DisplayName = "Refresh token with user not found should return 404 not found response.")]
    public async Task Handle_UserNotFound_ReturnsNotFoundFailure()
    {
        // Arrange
        var currentGuid = new Faker().Random.Guid();
        var command = new RefreshTokenCommand(
            currentGuid,
            new Faker().Random.Guid().ToString()
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<RefreshTokenCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário não encontrado.");
    }
}