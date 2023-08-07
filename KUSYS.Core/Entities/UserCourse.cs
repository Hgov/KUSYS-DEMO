using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Core.Entities
{
    public class UserCourse
    {
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public string CourseId { get; set; }
        public virtual Course course { get; set; }
    }
}
