using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("api/v1/auths")]
[OpenApiTags("Auths")]
[ApiExplorerSettings(GroupName = "Auths")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("")]
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

    [HttpGet]
    [Authorize]
    [Route("")]
    [ActionName("ValidatingAccessToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Authenticate()
        => Ok(Result<dynamic>.Success(null, messages: "Authenticated"));
}