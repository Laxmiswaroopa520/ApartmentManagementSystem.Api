using Microsoft.AspNetCore.Mvc;
// Enhanced with real-time data
namespace ApartmentManagementSystem.API.Controllers.V2
{
    public class DashboardApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
