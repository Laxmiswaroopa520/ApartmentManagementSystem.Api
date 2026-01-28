using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers.V2
{
    public class AdminResidentApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
