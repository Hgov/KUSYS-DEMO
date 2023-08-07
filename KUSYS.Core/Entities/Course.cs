using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Core.Entities
{
    [Table("Courses")]
    public class Course
    {
        public Course()
        {
            UserCourses = new HashSet<UserCourse>();
        }
        [Key]
        public string CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<UserCourse> UserCourses { get; set; }
    }
}
