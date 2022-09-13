using Forward.Business.Lib.Entity;
using Forward.UserManagementService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forward.UserManagementService.Services
{
    public enum SignUpResultError
    {
        UserNameTaken,
        SecretTooSimple
    }

    public enum ValidateResultError
    {
        CredentialTypeNotFound,
        CredentialNotFound,
        SecretNotValid
    }

    public class ValidateResult
    {
        public FWDUser User { get; set; }
        public bool Success { get; set; }
        public ValidateResultError? Error { get; set; }

        public ValidateResult(FWDUser user = null, bool success = false, ValidateResultError? error = null)
        {
            this.User = user;
            this.Success = success;
            this.Error = error;
        }
    }

    public enum ChangeSecretResultError
    {
        CredentialTypeNotFound,
        CredentialNotFound
    }

    public class ChangeSecretResult
    {
        public bool Success { get; set; }
        public ChangeSecretResultError? Error { get; set; }

        public ChangeSecretResult(bool success = false, ChangeSecretResultError? error = null)
        {
            this.Success = success;
            this.Error = error;
        }
    }

    public interface IUserManagementService
    {
        //sign on
        Task SignUpAsync(FWDUser user);
        Task AddProgramToUserRoleAsync(FWDUser user, string roleCode, string programId);
        Task RemoveProgramFromUserRoleAsync(FWDUser user, string roleCode, string programId);
        Task<FWDauthentication> SignInAsync(FWDRestData userData);
        Task SignOutAsync(HttpContext httpContext);
        
        //current user
        Task<int> GetCurrentUserIdAsync(HttpContext httpContext);
        Task<FWDUser> GetCurrentUserAsync(HttpContext httpContext);

        //manage user
        Task<FWDUser> GetUserAsync(string userName);
        Task UpdateUserAsync(FWDUser user);
        Task DeleteUserAsync(string userName);
        Task ChangeSecretAsync(string identifier, string currentSecret, string secret);

        //role
        Task<IEnumerable<FWDRestData>> GetAllRolesAsync();
        Task AddNewRoleAsync(FWDRole role);
        Task UpdateRoleAsync(FWDRole role);
        Task AddUserToRoleAync(string userName, string roleId);
        Task RemoveUserFromRoleAsync(string userName, string roleId);
    }
}