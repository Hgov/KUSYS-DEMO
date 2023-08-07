namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class EditUserModel
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime BirthDate { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<AssignRoleModel> Roles { get; set; } = new();
    }
}
