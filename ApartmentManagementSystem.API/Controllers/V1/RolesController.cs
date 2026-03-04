using ApartmentManagementSystem.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Provides API endpoints for retrieving system roles.
    /// 
    /// This controller is responsible for:
    /// - Fetching all available roles in the system.
    /// 
    /// Currently returns basic role information (Id and Name).
    /// </summary>
    [ApiController]
    // [ApiVersion("1.0")]  
    // [Route("api/v{version:apiVersion}/roles")]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        /// <summary>
        /// Repository responsible for role data access.
        /// </summary>
        private readonly IRoleRepository RoleRepo;

        /// <summary>
        /// Constructor for injecting role repository dependency.
        /// </summary>
        /// <param name="roleRepository">
        /// Repository that provides access to role entities.
        /// </param>
        public RolesController(IRoleRepository roleRepository)
        {
            RoleRepo = roleRepository;
        }

        /// <summary>
        /// Retrieves all roles available in the system.
        /// </summary>
        /// <returns>
        /// A list of roles containing:
        /// - Id
        /// - Name
        /// </returns>
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

















