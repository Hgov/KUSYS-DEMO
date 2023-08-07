using System.ComponentModel.DataAnnotations;

namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class ChangePasswordModel
    {
        public string Password { get; set; } = default!;
        public string NewPassword { get; set; } = default!;

        [Compare(nameof(NewPassword), ErrorMessage = "Entered passwords doesn't match.")]
        public string NewPasswordConfirm { get; set; } = default!;
    }
}
