using Forward.Business.Lib;
using Forward.Business.Lib.Entity;
using Newtonsoft.Json;

namespace Forward.UserManagementService.Models
{
    public class FWDUser : FWDRestData
    {
        public FWDUser() : base()
        {
            EntityType = Type;
        }

        public const string Type = "User";

        public string UserName { get { return Get1("UserName"); } set { Set1("UserName", value); } }
        public string Secret { get { return Get1("Secret"); } set { Set1("Secret", value); } }
        public string? Extra { get { return Get1("Extra"); } set { Set1("Extra", value); } }
        public string FirstName { get { return Get1("FirstName"); } set { Set1("FirstName", value); } }
        public string LastName { get { return Get1("Name"); } set { Set1("LastName", value); } }
        public string? MiddleName { get { return Get1("MiddleName"); } set { Set1("MiddleName", value); } }
        public string? SSN { get { return Get1Secure("SSN"); } set { Set1Secure("SSN", value); } }
        public string Phone { get { return Get1("Phone"); } set { Set1("Phone", value); } }
        public string Email { get { return Get1("Email"); } set { Set1("Email", value); } }
        public DateTime? LastLogin { get { return Get1DateTime("LastLogin"); } set { Set1("LastLogin", value); } }

        public List<FWDRole>? Roles
        {
            get
            {
                List<FWDRole> ret = new List<FWDRole>();
                List<IAttrExt> roles = GetAttrExt("FWDRole");
                foreach (IAttrExt ext in roles)
                {
                    FWDRole role = new FWDRole();
                    role.Attributes = ext.Extension;
                    ret.Add(role);
                }

                return ret;
            }
            set
            {
                RemoveForKey("FWDRole");
                foreach (FWDRole role in value)
                {
                    AttrExt ext = new AttrExt("FWDRole", string.Empty);
                    ext.Extension = role.Attributes;
                    Attributes.Add(ext);
                }
            }
        }

        public void AddRole(FWDRole role)
        {
            List<IAttrExt> roles = GetAttrExt("FWDRole");
            if (roles.Any(r => r.Extension.Get1("Name") == role.Name)) return;

            AttrExt ext = new AttrExt("FWDRole", string.Empty);
            ext.Extension = role.Attributes;
            Attributes.Add(ext);
        }
    }
}