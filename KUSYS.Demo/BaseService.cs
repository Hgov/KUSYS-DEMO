using AutoMapper;
using KUSYS.Core.Entities;
using KUSYS.Core.Helpers;
using KUSYS.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KUSYS.Demo
{
    public abstract class BaseService : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<Role> _roleManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected EmailHelper _emailHelper;
        public BaseService(IMapper mapper)
        {

            _mapper = mapper;
        }
        public BaseService(IMapper mapper, EmailHelper emailHelper)
        {
            _mapper = mapper;
            _emailHelper = emailHelper;
        }
        public BaseService(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IMapper mapper, EmailHelper emailHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailHelper = emailHelper;
        }

        public BaseService(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IMapper mapper, EmailHelper emailHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailHelper = emailHelper;
        }
    }
}
