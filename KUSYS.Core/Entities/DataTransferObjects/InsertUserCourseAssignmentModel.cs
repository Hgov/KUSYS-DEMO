using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class InsertUserCourseAssignmentModel
    {
        public string? userId { get; set; }
        public string? courseId { get; set; }
    }
}
