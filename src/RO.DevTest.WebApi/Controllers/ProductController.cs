using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("api/v1/products")]
[OpenApiTags("Products")]
[ApiExplorerSettings(GroupName = "Products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [Route("")]
    [ActionName("CreateProduct")]
    [ProducesResponseType(typeof(Result<CreateProductCommand>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CreateProductCommand>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CreateProductCommand>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        if (response.StatusCode is StatusCodes.Status201Created)
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        return StatusCode(response.StatusCode, response);
    }
}