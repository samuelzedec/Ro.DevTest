using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;
using RO.DevTest.Application.Features.User.Queries.GetUserById;
using RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmail;
using RO.DevTest.Domain.Abstract;
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
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserAdmin(
        [FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        request.Role = UserRoles.Admin;
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status400BadRequest)
            return BadRequest(response);
        
        if (response.StatusCode is StatusCodes.Status500InternalServerError)
            return StatusCode(response.StatusCode, response);
            
        return Created(HttpContext.Request.GetDisplayUrl(), response);
    }
    
    [HttpPost]
    [Route("customer")]
    [ActionName("CreateUserCustomer")]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserCustomer(
        [FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        request.Role = UserRoles.Customer;
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status400BadRequest)
            return BadRequest(response);
        
        if (response.StatusCode is StatusCodes.Status500InternalServerError)
            return StatusCode(response.StatusCode, response);
            
        return Created(HttpContext.Request.GetDisplayUrl(), response);
    }

    [HttpPut]
    [Authorize]
    [Route("")]
    [ActionName("UpdateUser")]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResult>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [Authorize]
    [Route("{id:guid}")]
    [ActionName("GetUserById")]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetUserByIdQuery(id);
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("")]
    [ActionName("GetUserByNameOrId")]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserByNameOrEmail(
        [FromQuery] string nameOrEmail,
        CancellationToken cancellationToken)
    {
        var request = new GetUserByNameOrEmailQuery(nameOrEmail);
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}