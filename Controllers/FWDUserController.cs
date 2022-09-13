using Microsoft.AspNetCore.Mvc;
using Forward.UserManagementService.Services;
using Forward.UserManagementService.Models;
using Forward.Business.Lib.Entity;
using Microsoft.AspNetCore.Authorization;
using Forward.Business.Lib.Controllers;
using Forward.CommonUtilities.FWDExceptions;

namespace Forward.UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FWDUserController : FWDBaseApiController
    {
        private readonly ILogger _logger;
        private readonly IUserManagementService _usermanagementService;

        public FWDUserController(IUserManagementService userService, ILogger<FWDRoleController> logger)
        {
            _usermanagementService = userService;
            _logger = logger;
        }

        // POST api/<UserController>/username
        [HttpPost("username")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<FWDRestData>> get(FWDRestData userName)
        {
            try
            {
                var user = await _usermanagementService.GetUserAsync(userName.Get1("username"));
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while retreiving data");
            }
        }

        // POST api/<UserController>/SignUp
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(FWDRestData data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FWDUser user = data.MakeSubTypeSpecific<FWDUser>();
                    await _usermanagementService.SignUpAsync(user);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while inserting data");
            }
        }

        // POST api/<UserController>/SignOn
        [HttpPost("signon")]
        public async Task<ActionResult<FWDauthentication>> SignOn(FWDRestData userData)
        {
            try
            {
                FWDauthentication token = await _usermanagementService.SignInAsync(userData);

                //return token in the end

                return Ok(token);
                //return Ok(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                
                if (ex is FWDInternalException)
                    return StatusCode((int)((FWDInternalException)ex).ErrorStatus, ex.Message);

                return StatusCode(500, "Some error occured while signing on");
            }
        }

        [HttpPut("updateuser")]
        public async Task<ActionResult> UpdateUser(FWDUser user)
        {
            try
            {
                await _usermanagementService.UpdateUserAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while updating data");
            }
        }
        
        // DELETE api/<UserController>/deleteUser
        [HttpDelete("deleteUser")]
        public async Task<ActionResult> DeleteUser(FWDRestData userData)
        {
            try
            {
                await _usermanagementService.DeleteUserAsync(userData.Get1("username"));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while deleting data");
            }
        }

        // POST api/<UserController>/changepw
        [HttpPost("changepw")]
        public async Task<ActionResult> UpdatePassword(FWDRestData userData)
        {
            try
            {
                await _usermanagementService.ChangeSecretAsync(userData.Get1("username"), userData.Get1("currentsecret"), userData.Get1("secret"));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while updating password");
            }
        }

    }
}
