using AutoMapper;
using KUSYS.Core.Entities;
using KUSYS.Core.Entities.DataTransferObjects;

namespace KUSYS.Infrastructure.MapperService
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<SignUpModel, User>();

            CreateMap<Role, UpsertRoleModel>();
            CreateMap<User, EditUserModel>();
            CreateMap<User, UpdateProfileModel>();

            CreateMap<Course, UpsertCourseModel>();
        }
    }
}
