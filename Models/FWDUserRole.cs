using Forward.Business.Lib.Entity;

namespace Forward.UserManagementService.Models
{
    /// <summary>
    /// this data is to help find all users with a particular role
    /// </summary>
    public class FWDUserRole : FWDRestData
    {
        public FWDUserRole() : base()
        {
            EntityType = Type;
        }

        public const string Type = "Role-to-User";

        public string UserName { get { return Get1("UserName"); } set { Set1("UserName", value); } }
        public string RoleId { get { return Get1("RoleId"); } set { Set1("RoleId", value); } }
    }
}
