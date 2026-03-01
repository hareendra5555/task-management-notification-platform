using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public HealthController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

        return Ok(new
        {
            status = "healthy",
            service = "taskflow-api",
            environment = _environment.EnvironmentName,
            version,
            timestampUtc = DateTime.UtcNow
        });
    }
}
