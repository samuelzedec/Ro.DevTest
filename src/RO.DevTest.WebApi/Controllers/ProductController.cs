using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;
using RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
using RO.DevTest.Application.Features.Product.Queries.GetAllProductsQuery;
using RO.DevTest.Application.Features.Product.Queries.GetProductQuery;
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
    [Route("")]
    [ActionName("GetAllProducts")]
    [ProducesResponseType(typeof(Result<List<GetAllProductsResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<GetAllProductsResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<List<GetAllProductsResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete]
    [Authorize]
    [Route("")]
    [ActionName("DeleteProduct")]
    [ProducesResponseType(typeof(Result<DeleteProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<DeleteProductResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<DeleteProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<DeleteProductResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(
        [FromQuery] DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}