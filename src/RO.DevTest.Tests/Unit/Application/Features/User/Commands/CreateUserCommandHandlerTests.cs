using Bogus;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using Npgsql;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<IValidator<CreateUserCommand>> _mockValidator;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockValidator = new Mock<IValidator<CreateUserCommand>>();
        _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();

        _handler = new CreateUserCommandHandler(
            _mockIdentityAbstractor.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Given valid user data with admin role, should create user successfully")]
    public async Task Handle_ValidCommandWithAdminRole_CreatesUserSuccessfully()
    {
        // Arrange
        var password = new Faker().Internet.Password(8);
        var command = new CreateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email(),
            password,
            password
        ) { Role = UserRoles.Admin };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.CreateUserAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

        _mockIdentityAbstractor
            .Setup(id => id.AddToRoleAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<UserRoles>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

        _mockIdentityAbstractor
            .Setup(id => id.GetUserRolesAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(new List<string>() { "Admin" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário criado com sucesso");
    }

    [Fact(DisplayName = "Given passwords that don't match, you should fall for the first \"if\" of validation")]
    public async Task Handle_PasswordsDoNotMatch_ReturnsValidationError()
    {
        // Arrange
        var command = new CreateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email(),
            new Faker().Internet.Password(),
            new Faker().Internet.Password()
        ) { Role = UserRoles.Admin };

        List<ValidationFailure> validationFailures = [new("PasswordConfirmation", "As senhas não conferem")];
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().ContainSingle().Which.Should().Be("As senhas não conferem");
    }

    [Fact(DisplayName = "Given database is unavailable, should log error and return server error")]
    public async Task Handle_DatabaseUnavailable_LogsErrorAndReturnsServerError()
    {
        // Arrange
        var password = new Faker().Internet.Password(8);
        var command = new CreateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email(),
            password,
            password
        ) { Role = UserRoles.Admin };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<CreateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockIdentityAbstractor
            .Setup(id => id.CreateUserAsync(
                It.IsAny<Domain.Entities.Identity.User>(),
                It.IsAny<string>()))
            .ThrowsAsync(new NpgsqlException("No connection could be made because the target machine actively refused it."));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().ContainSingle().Which.Should()
            .Be("No connection could be made because the target machine actively refused it.");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No connection could be made because the target machine actively refused it.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}