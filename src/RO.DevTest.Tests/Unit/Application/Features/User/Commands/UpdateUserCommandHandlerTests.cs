using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IValidator<UpdateUserCommand>> _mockValidator;
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _mockLogger;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockValidator = new Mock<IValidator<UpdateUserCommand>>();
        _mockLogger = new Mock<ILogger<UpdateUserCommandHandler>>();

        _handler = new UpdateUserCommandHandler(
            _mockIdentityAbstractor.Object,
            _mockCurrentUserService.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Handle should successfully update user when provided with valid user information")]
    public async Task Handle_ValidUser_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var command = new UpdateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email()
        );

        var currentUserId = new Faker().Random.Guid();
        var currentUser = new Domain.Entities.Identity.User
        {
            Id = currentUserId,
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email()
        };

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(currentUserId.ToString());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(currentUser);

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        _mockIdentityAbstractor
            .Setup(id => id.UpdateUserAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Dados atualizados com sucesso");
    }

    [Fact(DisplayName = "Handle should return failure result when attempting to update non-existent user")]
    public async Task Handle_UserNotFound_ShouldFailUpdate()
    {
        // Arrange
        var command = new UpdateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email()
        );

        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(string.Empty);
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário não encontrado");
    }

    [Fact(DisplayName = "Handle should return failure when attempting to update user with email already registered to another account")]
    public async Task Handle_EmailAlreadyExists_ShouldFailUpdate()
    {
        // Arrange
        var command = new UpdateUserCommand(
            new Faker().Internet.UserName(),
            new Faker().Name.FullName(),
            new Faker().Internet.Email()
        );

        var currentUserId = new Faker().Random.Guid();
        var currentUser = new Domain.Entities.Identity.User
        {
            Id = currentUserId,
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email()
        };
        
        var otherUser = new Domain.Entities.Identity.User
        {
            Id = new Faker().Random.Guid(),
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email()
        };
        
        _mockValidator
            .Setup(va => va.ValidateAsync(
                It.IsAny<UpdateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(currentUserId.ToString());
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);
        
        _mockIdentityAbstractor
            .Setup(id => id.FindUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(otherUser);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().ContainSingle().Which.Should().Be("Informações existentes");
    }
}