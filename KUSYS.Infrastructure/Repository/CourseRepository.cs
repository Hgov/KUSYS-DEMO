using KUSYS.Core.Entities;
using KUSYS.Core.Helpers;
using KUSYS.Core.Repository;

namespace KUSYS.Infrastructure.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(DataContext dataContext) : base(dataContext)
        {

        }
        public DataContext dataContext { get { return _Context as DataContext; } }
    }
}
