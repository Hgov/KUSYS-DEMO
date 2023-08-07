using KUSYS.Core.Entities;

namespace KUSYS.Core.Repository
{
    public interface IUserCourseRepository : IRepository<UserCourse>
    {
        Task<bool> GetExistingUserCourseAsync(string? userId,string? courseId);
        Task<UserCourse> GetByIdUserCourseAsync(string? userId, string? courseId);
    }
}
