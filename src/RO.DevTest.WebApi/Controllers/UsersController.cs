using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;
using RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;
using RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmailQuery;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("/v1/users")]
[OpenApiTags("Users")]
[ApiExplorerSettings(GroupName = "Users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("admin")]
    [ActionName("CreateUserAdmin")]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateUserResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [Authorize]
    [Route("")]
    [ActionName("GetUserById")]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("{nameOrEmail}")]
    [ActionName("GetUserByNameOrId")]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetUserByIdResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserByNameOrEmail(
        [FromRoute] string nameOrEmail,
        CancellationToken cancellationToken)
    {
        var request = new GetUserByNameOrEmailQuery(nameOrEmail);
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}