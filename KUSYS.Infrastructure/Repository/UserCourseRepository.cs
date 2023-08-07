using KUSYS.Core.Entities;
using KUSYS.Core.Helpers;
using KUSYS.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Infrastructure.Repository
{
    public class UserCourseRepository : Repository<UserCourse>, IUserCourseRepository
    {
        public UserCourseRepository(DataContext dataContext) : base(dataContext)
        {

        }
        public DataContext dataContext { get { return _Context as DataContext; } }

        public async Task<bool> GetExistingUserCourseAsync(string? userId, string? courseId)
        {
           return await _Context.UserCourses.Where(x=>x.User.Id==userId&&x.course.CourseId==courseId).AnyAsync();
        }
        public async Task<UserCourse> GetByIdUserCourseAsync(string? userId, string? courseId)
        {
            return await _Context.UserCourses.Where(x => x.User.Id == userId && x.course.CourseId == courseId).FirstOrDefaultAsync();
        }
    }
}
