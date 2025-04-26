using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Queries;

public class GetUserByIdHandlerQueryTests
{
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<GetUserByIdQueryHandler>> _mockLogger;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdHandlerQueryTests()
    {
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<GetUserByIdQueryHandler>>();

        _handler = new GetUserByIdQueryHandler(
            _mockIdentityAbstractor.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Should return success result when user exists")]
    public async Task Handle_WhenUserExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new GetUserByIdQuery();
        var currentGuid = new Faker().Random.Guid();
        var existingCurrentUser = new Domain.Entities.Identity.User
        {
            Id = currentGuid,
            UserName = new Faker().Internet.UserName(),
            Name = new Faker().Name.FullName(),
            Email = new Faker().Internet.Email(),
        };

        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(currentGuid.ToString());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existingCurrentUser);

        _mockIdentityAbstractor
            .Setup(id => id.GetUserRolesAsync(It.IsAny<Domain.Entities.Identity.User>()))
            .ReturnsAsync(["Customer"]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário encontrado");
    }

    [Fact(DisplayName = "Handle should return failure result when user does not exist")]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var query = new GetUserByIdQuery();
        
        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(string.Empty);

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Identity.User)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Usuário não encontrado");
    }

    [Fact(DisplayName = "Handle should return error result when database connection fails")]
    public async Task Handle_WhenDatabaseUnavailable_ShouldReturnInternalServerError()
    {
        // Arrange
        var query = new GetUserByIdQuery();
        
        _mockCurrentUserService
            .Setup(cs => cs.GetCurrentUserId())
            .Returns(new Faker().Random.Guid().ToString());

        _mockIdentityAbstractor
            .Setup(id => id.FindUserByIdAsync(It.IsAny<string>()))
            .Throws(new NpgsqlException("No connection could be made because the target machine actively refused it."));
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert 
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Data.Should().BeNull();
        result.Message.Should().ContainSingle().Which.Should().Be("Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        
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