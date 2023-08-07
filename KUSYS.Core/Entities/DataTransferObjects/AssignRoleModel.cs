namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class AssignRoleModel
    {
        public string RoleId { get; set; } = default!;
        public string RoleName { get; set; } = default!;
        public bool IsAssigned { get; set; }
    }
}
