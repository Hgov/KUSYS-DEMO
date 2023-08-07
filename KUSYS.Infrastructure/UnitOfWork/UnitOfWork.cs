using KUSYS.Core.Helpers;
using KUSYS.Core.Repository;
using KUSYS.Core.UnitOfWork;
using KUSYS.Infrastructure.Repository;

namespace KUSYS.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private DataContext _DataContext;

        public UnitOfWork(DataContext DataContext)
        {
            _DataContext = DataContext;

            CourseRepository = new CourseRepository(_DataContext);
            UserCourseRepository = new UserCourseRepository(_DataContext);
        }

        public ICourseRepository CourseRepository { get; private set; }

        public IUserCourseRepository UserCourseRepository { get; private set; }

        public int Complete()
        {

            return _DataContext.SaveChanges();
        }

        public void Dispose()
        {
            _DataContext.Dispose();
        }
    }
}
