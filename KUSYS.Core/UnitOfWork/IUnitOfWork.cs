using KUSYS.Core.Repository;

namespace KUSYS.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository CourseRepository { get; }
        IUserCourseRepository UserCourseRepository { get; }
        int Complete();
        void Dispose();
    }
}
