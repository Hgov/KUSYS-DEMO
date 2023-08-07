using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KUSYS.Core.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            UserCourses = new HashSet<UserCourse>();
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<UserCourse> UserCourses { get; set; }
    }
}