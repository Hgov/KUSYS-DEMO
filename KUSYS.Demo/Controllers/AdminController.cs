using AutoMapper;
using KUSYS.Core.Entities;
using KUSYS.Core.Entities.DataTransferObjects;
using KUSYS.Core.Helpers;
using KUSYS.Core.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Demo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseService
    {
        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IMapper mapper, EmailHelper emailHelper) : base(userManager, roleManager, signInManager, unitOfWork, mapper, emailHelper)
        {

        }
        public IActionResult Index() => View();
        public async Task<IActionResult> Users() => View(await _userManager.Users.ToListAsync());
        [AllowAnonymous]
        public async Task<IActionResult> Roles() => View(await _roleManager.Roles.ToListAsync());
        public async Task<IActionResult> Courses() => View(await _unitOfWork.CourseRepository.GetAllAsync());
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var assignRolModel = new List<AssignRoleModel>();
            foreach (var item in roles)
            {
                assignRolModel.Add(new AssignRoleModel
                {
                    RoleName = item.Name,
                    IsAssigned = false,
                    RoleId = item.Id
                });
            }
            var signUpModel = new SignUpModel { Roles = assignRolModel };
            return View(signUpModel);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(SignUpModel signUpModel)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(signUpModel);
                var result = await _userManager.CreateAsync(user, signUpModel.Password);
                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Admin", new
                    {
                        userId = user.Id,
                        token = confirmationToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Confirm e-mail",
                        Body = $"Hello! For the {user.FirstName + " " + user.LastName} user. Please <a href='{confirmationLink}'>click</a> to confirm your e-mail address.",
                        To = user.Email
                    });


                    foreach (var item in signUpModel.Roles)
                    {
                        if (item.IsAssigned)
                        {
                            await _userManager.AddToRoleAsync(user, item.RoleName);
                        }
                    }
                    return RedirectToAction("Users");

                }
                result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
            }
            return View(signUpModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }
            return RedirectToAction("Users");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "User");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public async Task<IActionResult> UpsertRole(string? id)
        {
            if (id != null)
            {
                var role = await _roleManager.FindByIdAsync(id);
                return View(_mapper.Map<UpsertRoleModel>(role));
            }
            return View(new UpsertRoleModel());
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpsertRole(UpsertRoleModel upsertRoleModel)
        {
            //eğer rol modeli üzerinde id alanı dolu ise güncelle, boş ise yeni rol ekle
            if (ModelState.IsValid)
            {
                var isUpdate = upsertRoleModel.Id != null;

                var role = isUpdate ? await _roleManager.FindByIdAsync(upsertRoleModel.Id) : new Role() { Name = upsertRoleModel.Name };

                if (isUpdate)
                {
                    role.Name = upsertRoleModel.Name;
                }

                var result = isUpdate ? await _roleManager.UpdateAsync(role) : await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
            }
            return View(upsertRoleModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Role not found.");
            }
            return RedirectToAction("Roles");
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Users");
            }
            var userModel = _mapper.Map<EditUserModel>(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            userModel.Roles = await _roleManager.Roles.Select(s => new AssignRoleModel
            {
                RoleId = s.Id,
                RoleName = s.Name,
                IsAssigned = userRoles.Any(a => a == s.Name)
            }).ToListAsync();

            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel editUserModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUserModel.Id);
                if (user != null)
                {
                    user.FirstName = editUserModel.FirstName;
                    user.LastName = editUserModel.LastName;
                    user.BirthDate = editUserModel.BirthDate;
                    user.UserName = editUserModel.UserName;
                    user.Email = editUserModel.Email;

                    var result = await _userManager.UpdateAsync(user);//seçili kullanıcının güncellenmesi
                    if (result.Succeeded)
                    {
                        foreach (var item in editUserModel.Roles)//seçili olan roller listesi
                        {
                            if (item.IsAssigned)
                            {
                                await _userManager.AddToRoleAsync(user, item.RoleName);//kullanıcıya rol ekleme işlemi
                            }
                            else
                            {
                                await _userManager.RemoveFromRoleAsync(user, item.RoleName);//kullanıcı üzerinden rol kaldırma işlemi
                            }
                        }
                        return RedirectToAction("Users");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(editUserModel);
        }
        public async Task<IActionResult> UpsertCourse(string? id)
        {
            if (id != null)
            {
                var course = await _unitOfWork.CourseRepository.GetByIDAsync(id);
                return View(_mapper.Map<UpsertCourseModel>(course));
            }
            return View(new UpsertCourseModel());
        }
        [HttpPost]
        public async Task<IActionResult> UpsertCourse(UpsertCourseModel upsertCourseModel)
        {
            if (ModelState.IsValid)
            {
                var isUpdate = upsertCourseModel.Id != null;

                var course = isUpdate ? await _unitOfWork.CourseRepository.GetByIDAsync(upsertCourseModel.Id.ToString()) : new Course() { Name = upsertCourseModel.Name };
                if (!isUpdate)
                    course.CourseId = Guid.NewGuid().ToString();

                if (!string.IsNullOrWhiteSpace(upsertCourseModel.CourseCode))
                    course.CourseCode = upsertCourseModel.CourseCode;
                if (!string.IsNullOrWhiteSpace(upsertCourseModel.Name))
                    course.Name = upsertCourseModel.Name;

                var result = isUpdate ? _unitOfWork.CourseRepository.Update(course) : await _unitOfWork.CourseRepository.AddAsync(course);
                if (result != null)
                {
                    _unitOfWork.Complete();
                    return RedirectToAction("Courses");
                }
            }
            return View(upsertCourseModel);
        }
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var course = await _unitOfWork.CourseRepository.GetByIDAsync(id);
            if (course != null)
            {
                _unitOfWork.CourseRepository.Remove(course);
                _unitOfWork.Complete();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Course not found.");
            }
            return RedirectToAction("Courses");
        }
    }
}
