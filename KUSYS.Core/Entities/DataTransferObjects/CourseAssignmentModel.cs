using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KUSYS.Core.Entities.DataTransferObjects
{
    public class CourseAssignmentModel
    {
        public string? CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? Name { get; set; }
        public bool IsAssignment { get; set; }
    }
}
