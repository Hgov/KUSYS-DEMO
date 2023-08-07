namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
