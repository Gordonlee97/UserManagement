using Forward.UserManagementService.Models;
using Forward.UserManagementService.Services;
using Microsoft.AspNetCore.Mvc;
using Forward.Business.Lib.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Forward.UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FWDRoleController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly IUserManagementService _userService;

        public FWDRoleController(IUserManagementService userService, ILogger<FWDRoleController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<FWDRestData>>> GetAllRoles()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while retreiving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewRole(FWDRestData data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FWDRole role = data.MakeSubTypeSpecific<FWDRole>();
                    await _userService.AddNewRoleAsync(role);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while inserting data");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRole(FWDRestData data)
        {
            try
            {
                FWDRole role = data.MakeSubTypeSpecific<FWDRole>();
                await _userService.UpdateRoleAsync(role);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while updating data");
            }
        }
    }
}
