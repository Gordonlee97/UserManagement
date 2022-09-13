using Forward.Business.Lib.Entity;

namespace Forward.UserManagementService.Models
{
    public class FWDRole : FWDRestData
    {
        public FWDRole() : base()
        {
            EntityType = Type;
        }

        public const string Type = "Role";
        public override string? PartitionKey => Type;
        public List<string> Permissions {get { return Get("Permissions"); } set { Set("Permissions", value); } }
    }
}