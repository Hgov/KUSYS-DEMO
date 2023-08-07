using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [MinimumElements(1)]
        public List<AssignRoleModel> Roles { get; set; }
    }
    public class MinimumElementsAttribute : ValidationAttribute
    {
        private readonly int minElements;

        public MinimumElementsAttribute(int minElements)
        {
            this.minElements = minElements;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;
            var assignRoleModel = new List<AssignRoleModel>();
            if (list == null) return ValidationResult.Success;
            foreach (var item in list)
            {
                assignRoleModel.Add((AssignRoleModel)item);
            }
            var result = assignRoleModel.Where(x => x.IsAssigned).Count() >= minElements;

            return result
                ? ValidationResult.Success
                : new ValidationResult($"{validationContext.DisplayName} requires at least {minElements} element" + (minElements > 1 ? "s" : string.Empty));
        }
    }
}
