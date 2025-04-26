using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("/v1/auth")]
[OpenApiTags("Auth")]
[ApiExplorerSettings(GroupName = "Auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("")]
    [ActionName("ValidatingAccessToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Authenticate()
        => Ok(Result<dynamic>.Success(null, messages: "Autenticado"));
    
    [HttpPost]
    [Route("login")]
    [ActionName("LoginAsync")]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Route("refresh-token")]
    [ActionName("VerifyRefreshToken")]
    [ProducesResponseType(typeof(Result<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<RefreshTokenResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<RefreshTokenResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyRefreshToken(
        [FromBody] RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}