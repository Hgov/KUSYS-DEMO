namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class UpdateProfileModel
    {
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public DateTime? BirthDate { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
