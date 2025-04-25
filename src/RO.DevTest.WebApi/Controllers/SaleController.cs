using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;
using RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;
using RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;
using RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;
using RO.DevTest.Application.Features.Sale.Queries.GetProductSalesByAdminQuery;
using RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;
using RO.DevTest.Application.Features.Sale.Queries.GetSalesByPeriodQuery;
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
    
    [HttpDelete]
    [Authorize]
    [Route("")]
    [ActionName("DeleteSale")]
    [ProducesResponseType(typeof(Result<DeleteSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<DeleteSaleResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<DeleteSaleResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSale(
        [FromBody] DeleteSaleCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("customer")]
    [ActionName("GetMyPurchases")]
    [ProducesResponseType(typeof(Result<List<GetMyPurchasesResponse>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<List<GetMyPurchasesResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<List<GetMyPurchasesResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyPurchases(
        [FromQuery] GetMyPurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("{saleId:guid}")]
    [ActionName("GetSaleById")]
    [ProducesResponseType(typeof(Result<GetSaleByIdResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<GetSaleByIdResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetSaleByIdResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSaleById(
        [FromRoute] Guid saleId,
        CancellationToken cancellationToken)
    {
        var request = new GetSaleByIdQuery(saleId);
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("admin")]
    [ActionName("GetAdminSales")]
    [ProducesResponseType(typeof(Result<List<GetSalesByPeriodResponse>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<List<GetSalesByPeriodResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<List<GetSalesByPeriodResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdminSales(
        [FromQuery] GetSalesByPeriodQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("admin/analysis")]
    [ActionName("GetSalesAnalisys")]
    [ProducesResponseType(typeof(Result<List<GetProductSalesByAdminResponse>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<List<GetProductSalesByAdminResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<List<GetProductSalesByAdminResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSalesAnalisys(
        [FromQuery] GetProductSalesByAdminQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}