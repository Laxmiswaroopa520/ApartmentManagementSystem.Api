using ApartmentManagementSystem.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]  
    [Route("api/v{version:apiVersion}/roles")]
    //[Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository RoleRepo;

        public RolesController(IRoleRepository roleRepository)
        {
            RoleRepo = roleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await RoleRepo.GetAllAsync();

            var result = roles.Select(r => new
            {
                Id = r.Id,
                Name = r.Name
            });

            return Ok(result);
        }
    }

}
