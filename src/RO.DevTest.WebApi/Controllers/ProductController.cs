using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
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
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<UpdateProductResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}