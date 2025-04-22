using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;
using RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("/v1/sales")]
[OpenApiTags("Sales")]
[ApiExplorerSettings(GroupName = "Sales")]
public class SaleController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [Route("")]
    [ActionName("CreateSale")]
    [ProducesResponseType(typeof(Result<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateSaleResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateSaleResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleCommand request,
        CancellationToken cancellationToken)
    {
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
    [ActionName("UpdateSale")]
    [ProducesResponseType(typeof(Result<UpdateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<UpdateSaleResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<UpdateSaleResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSale(
        [FromBody] UpdateSaleCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}