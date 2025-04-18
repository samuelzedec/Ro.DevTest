using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("/api")]
[OpenApiTags("Health System")]
[ApiExplorerSettings(GroupName = "Monitoring and Diagnostics")]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [OpenApiOperation("Check Availability", "Confirms if the system is operational")]
    public IActionResult CheckAvailability()
        => Ok(Result<dynamic>.Success(null, messages: "System is operational and responding normally"));
}