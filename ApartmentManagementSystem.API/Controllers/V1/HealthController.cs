using Microsoft.AspNetCore.Mvc;


namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Apartment Management System API"
        });
    }
}