using Forward.CommonUtilities.StringUtils;
using Forward.UserManagementService.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Forward.Business.Lib.FWDStore;
using Forward.Business.Lib;
using Forward.Business.Lib.Entity;
using Forward.Business.Lib.Common.Security;
using Forward.Business.Lib.FWDCache;
using Forward.Business.Lib.Interfaces;
using Forward.CommonUtilities.ThreadUtils;
using Forward.CommonUtilities.FWDExceptions;
using System.Net;

namespace Forward.UserManagementService.Services
{
    public class UserManagementService : IUserManagementService
    {
        #region private member
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        #endregion

        #region sign on
        public async Task SignUpAsync(FWDUser user)
        {
            user.Created = DateTime.Now;

            //TODO: validate username, if taken, return error
            //TODO: validate secret, return error if it is too simple, later will store previous used secrets and check
            //TODO: validate roles
            //TODO: encrypted user
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(user.Secret, salt);

            user.Secret = hash;
            user.Extra = Convert.ToBase64String(salt);

            user.Id = $"{FWDUser.Type}_{user.UserName}";
            user.PartitionKey = user.Id;
            await FWDStoreAccess.CreateAsync(user);
        }

        public async Task<FWDauthentication> SignInAsync(FWDRestData userData)
        {
            //TODO
            //ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            //var user = await GetUserAsync(userName);

            FWDUser user = null;

            try
            {
                user = await FWDStoreAccess.GetAsync<FWDUser>($"{FWDUser.Type}_{userData.Get1("username")}");
            }
            catch(Exception ex)
            {
                if (!(ex is FWDInternalException) || ((FWDInternalException)ex).ErrorStatus == HttpStatusCode.NotFound)
                    throw new FWDInternalException("Log in failed.", System.Net.HttpStatusCode.BadRequest);

                throw;
            }

            string hash = Pbkdf2Hasher.ComputeHash(userData.Get1("secret"), Convert.FromBase64String(user.Extra));

            bool signinSuccessful = hash == user.Secret;

            if (!signinSuccessful)
                throw new FWDInternalException("Log in failed.", System.Net.HttpStatusCode.BadRequest);

            //create a token here
            FWDauthentication token = await CreateTokenAsync(user, signinSuccessful);

            return token;
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            //TODO -- expire JWT token
            //await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task<FWDauthentication> CreateTokenAsync(FWDUser user, bool loginSuccessful)
        {
            var authenticationModel = new FWDauthentication();

            authenticationModel.UserName = user.UserName;
            authenticationModel.Email = user.Email;
            authenticationModel.IsAuthenticated = loginSuccessful;

            authenticationModel.Token = loginSuccessful? BuildToken(user) : String.Empty;
          
            return authenticationModel;
        }

        private string BuildToken(FWDUser user)
        {
            IConfiguration config = CacheManager.AppSettings;

            //var userSerialise = JsonConvert.SerializeObject(user);

            var username = user.UserName;
            var expdate = DateTime.UtcNow.AddHours(1);
            var email = user.Email;

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Email, user.Email),
            });

            return TokenGV.GenerateToken(claims, 15);
        }

        #endregion


        #region manage user

        public async Task AddProgramToUserRoleAsync(FWDUser user, string roleCode, string programId)
        {

        }

        public async Task RemoveProgramFromUserRoleAsync(FWDUser user, string roleCode, string programId)
        {

        }

        public async Task AddUserToRoleAync(string userName, string roleId)
        {
            //TODO
            //FWDRole role = this.storage.Roles.FirstOrDefault(r => r.Code.ToLower() == roleCode.ToLower());

            //if (role == null)
            //  return;

            //this.AddToRole(user, role);
        }

        public async Task RemoveUserFromRoleAsync(string userName, string roleId)
        {

        }

        public async Task ChangeSecretAsync(string identifier, string currentSecret, string secret)
        {
            byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
            string hash = Pbkdf2Hasher.ComputeHash(secret, salt);

            var user = await GetUserAsync(identifier);
            string currentSecretHash = Pbkdf2Hasher.ComputeHash(currentSecret, Convert.FromBase64String(user.Extra));
            if (user.Secret != currentSecretHash) throw new Exception("Forbidden");

            user.Secret = hash;
            user.Extra = Convert.ToBase64String(salt);

            await FWDStoreAccess.UpdateAsync(user);
        }

        public async Task<FWDUser> GetUserAsync(string userName)
        {
            return await FWDStoreAccess.GetAsync<FWDUser>($"{FWDUser.Type}_{userName}");
        }

        public async Task UpdateUserAsync(FWDUser user)
        {
            var existingUser = await GetUserAsync(user.Id);
            //do not change secret when update user, call change secret api if need to update secret
            user.Secret = existingUser.Secret;
            await FWDStoreAccess.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(string userName)
        {
            var user = await GetUserAsync(userName);
            user.pstate = 0;
            await FWDStoreAccess.UpdateAsync(user);
        }

        private IEnumerable<Claim> GetUserClaims(FWDUser user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetUserRoleClaims(FWDUser user)
        {
            List<Claim> claims = new List<Claim>();
            //TODO get claims for all roles
            return claims;
        }

        #endregion

        #region role

        public async Task<IEnumerable<FWDRestData>> GetAllRolesAsync()
        {
            string queryString = @$"SELECT * FROM c WHERE c.PartitionKey = '{FWDRole.Type}'";
            StoreOption option = new StoreOption();
            option.PartitionKey = FWDRole.Type;
            option.Set1("MaxItemCount", 10);
            option.Set1("MaxConcurrency", 1);

            return await FWDStoreAccess.GetListFromQueryAsync<FWDRestData>(queryString, option: option);
        }

        public async Task AddNewRoleAsync(FWDRole role)
        {
            role.Id = $"{FWDRole.Type}_{role.Name}";

            try
            {
                await FWDStoreAccess.CreateAsync(role);
            }
            catch(Exception ex)
            {
                string err = ex.Message;
            }
        }

        public async Task UpdateRoleAsync(FWDRole role)
        {
            if (role == null) return;

            if (await GetRoleFromId(role.Id))
            {
                await FWDStoreAccess.UpdateAsync(role);
            }
        }

        private async Task<bool> GetRoleFromId(string roleId)
        {
            //use parameterized query to avoid sql injection
            string query = $"select * from c where c.PartitionKey = '{FWDRole.Type}' AND c.id=@roleId";
            StoreOption option = new StoreOption();
            option.PartitionKey = FWDRole.Type;
            option.Set1("MaxItemCount", 10);
            option.Set1("MaxConcurrency", 1);

            List<FWDRole> roles = await FWDStoreAccess.GetListFromQueryAsync<FWDRole>(query, option: option);

            return roles.Count > 0;
        }



        #endregion

        #region current user
        public async Task<int> GetCurrentUserIdAsync(HttpContext httpContext)
        {
            //TODO
            throw new NotImplementedException();
        }

        public async Task<FWDUser> GetCurrentUserAsync(HttpContext httpContext)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

        #region helper

        private static T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                    {
                        return Serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }

        #endregion
    }
}