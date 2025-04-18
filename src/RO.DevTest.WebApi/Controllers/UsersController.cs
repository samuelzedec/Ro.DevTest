using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("api/v1/users")]
[OpenApiTags("Users")]
[ApiExplorerSettings(GroupName = "Users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("admin")]
    [ActionName("CreateUserAdmin")]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserAdmin(
        [FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        request.Role = UserRoles.Admin;
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status400BadRequest)
            return BadRequest(response);
            
        return Created(HttpContext.Request.GetDisplayUrl(), response);
    }
    
    [HttpPost]
    [Route("customer")]
    [ActionName("CreateUserCustomer")]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserCustomer(
        [FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        request.Role = UserRoles.Customer;
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status400BadRequest)
            return BadRequest(response);
            
        return Created(HttpContext.Request.GetDisplayUrl(), response);
    }
}