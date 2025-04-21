using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
using RO.DevTest.Application.Features.Product.Queries.GetProductQuery;
using RO.DevTest.Application.Features.Product.Queries.GetProductsByAdminIdQuery;
using RO.DevTest.Application.Features.Product.Queries.GetProductsByCategoryQuery;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("/v1/products")]
[OpenApiTags("Products")]
[ApiExplorerSettings(GroupName = "Products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [Route("")]
    [ActionName("CreateProduct")]
    [ProducesResponseType(typeof(Result<CreateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateProductResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateProductResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status201Created)
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    [Authorize]
    [Route("")]
    [ActionName("UpdateProduct")]
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [Authorize]
    [Route("{productId:guid}")]
    [ActionName("GetProduct")]
    [ProducesResponseType(typeof(Result<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<GetProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<GetProductResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProductQuery(productId), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("me")]
    [ActionName("GetProductByAdminId")]
    [ProducesResponseType(typeof(Result<List<GetProductsByAdminIdResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<GetProductsByAdminIdResponse>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<List<GetProductsByAdminIdResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductByAdminId(
        [FromQuery] GetProductsByAdminIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet]
    [Authorize]
    [Route("")]
    [ActionName("GetProductByCategory")]
    [ProducesResponseType(typeof(Result<List<GetProductsByCategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<GetProductsByCategoryResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<List<GetProductsByCategoryResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductByCategory(
        [FromQuery] GetProductsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}