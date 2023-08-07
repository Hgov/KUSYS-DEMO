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
    [Authorize(Roles = "Admin,Student")]
    public class UserController : BaseService
    {
        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IMapper mapper, EmailHelper emailHelper) : base(userManager, roleManager, signInManager, unitOfWork, mapper, emailHelper)
        {

        }

        public async Task<IActionResult> Users() => View(await _userManager.Users.ToListAsync());
        public async Task<IActionResult> ViewUser(string id)
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
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            if (returnUrl != null)
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(signInModel.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, signInModel.Password, signInModel.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);
                        var returnUrl = TempData["ReturnUrl"];
                        if (returnUrl != null)
                        {
                            return Redirect(returnUrl.ToString() ?? "/");
                        }
                        return RedirectToAction("index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutEndUtc = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutEndUtc.Value - DateTime.UtcNow;
                        ModelState.AddModelError(string.Empty, $"This account has been locked out, please try again {timeLeft.Minutes} minutes later.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "You need to confirm your e-mail address.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid e-mail or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid e-mail or password.");
                }
            }
            return View(signInModel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(_mapper.Map<UpdateProfileModel>(user));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileModel updateProfileModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                if (user != null)
                {

                    user.FirstName = updateProfileModel.FirstName;
                    user.LastName = updateProfileModel.LastName;
                    user.BirthDate = updateProfileModel.BirthDate;
                    user.UserName = updateProfileModel.UserName;
                    user.Email = updateProfileModel.Email;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, true);

                        return RedirectToAction("Profile", "User");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));

                }
                else
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(updateProfileModel);
        }
        [AllowAnonymous]
        public IActionResult ChangePassword() => View();
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);

                var passwordValid = await _userManager.CheckPasswordAsync(user, changePasswordModel.Password);
                if (passwordValid)
                {
                    var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.Password, changePasswordModel.NewPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, true);

                        return RedirectToAction("Profile", "User");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password is invalid.");
                }
            }

            return View();
        }
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user != null)
                {
                    var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordLink = Url.Action("ResetPassword", "User", new
                    {
                        userId = user.Id,
                        token = passwordResetToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Reset password",
                        Body = $"Please <a href='{passwordLink}'>click</a> to reset your password.",
                        To = user.Email
                    });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(forgotPasswordModel);
        }
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ResetPasswordModel
            {
                UserId = userId,
                Token = token
            });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        return RedirectToAction("Login", "User");
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(resetPasswordModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> UserCourseAssignment(string Id)
        {
            ViewData["UserId"] = Id;
            var selectedUser = _userManager.Users.Where(x => x.Id == Id).FirstOrDefault();
            if (selectedUser != null)
                ViewData["FullName"] = selectedUser.FirstName + " " + selectedUser.LastName;

            var isRoleAdmin = false;
            foreach (var item in User.Claims)
            {
                if (item.Type.Contains("role"))
                    isRoleAdmin = item.Value.Equals("Admin") ? true : false;
            }

            var userCourseAssignmentModel = new UserCourseAssignmentModel();
            var userAssignmentModel = new List<UserAssignmentModel>();
            var courseAssignmentModel = new List<CourseAssignmentModel>();
            if (isRoleAdmin)
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var item in users)
                {
                    var roles = await _userManager.GetRolesAsync(item);
                    if (!roles.Where(x => x == "Admin").Any())
                    {
                        userAssignmentModel.Add(new UserAssignmentModel
                        {
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            FullName = item.FirstName + " " + item.LastName,
                            Id = item.Id,
                        });
                    };

                }
                userCourseAssignmentModel.userAssignmentModel = userAssignmentModel;

            }
            else
            {
                var user = await _userManager.GetUserAsync(User);

                userAssignmentModel.Add(new UserAssignmentModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FirstName + " " + user.LastName,
                    Id = user.Id,
                });
                userCourseAssignmentModel.userAssignmentModel = userAssignmentModel;
            }

            var courses = await _unitOfWork.CourseRepository.GetAllAsync();
            foreach (var item in courses)
            {
                var isAssignment = await _unitOfWork.UserCourseRepository.GetExistingUserCourseAsync(Id, item.CourseId);
                courseAssignmentModel.Add(new CourseAssignmentModel
                {
                    CourseCode = item.CourseCode,
                    CourseId = item.CourseId,
                    Name = item.Name,
                    IsAssignment = isAssignment
                });
            }

            userCourseAssignmentModel.courseAssignmentModel = courseAssignmentModel;

            return View(userCourseAssignmentModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserCourseAssignment(InsertUserCourseAssignmentModel insertUserCourseAssignmentModel)
        {
            var selectedUser = await _userManager.Users.Where(x => x.Id == insertUserCourseAssignmentModel.userId).FirstOrDefaultAsync();
            var selectedCourse = await _unitOfWork.CourseRepository.GetByIDAsync(new Guid(insertUserCourseAssignmentModel.courseId).ToString());
            var userCourse = new UserCourse
            {
                course = selectedCourse,
                User = selectedUser
            };
            var isExisting = await _unitOfWork.UserCourseRepository.GetExistingUserCourseAsync(userCourse.User.Id, userCourse.course.CourseId);
            if (!isExisting)
            {
                _unitOfWork.UserCourseRepository.AddAsync(userCourse);
                _unitOfWork.Complete();
            }
            return RedirectToAction();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RemoveUserCourseAssignment(InsertUserCourseAssignmentModel insertUserCourseAssignmentModel)
        {
            var userCourse = await _unitOfWork.UserCourseRepository.GetByIdUserCourseAsync(insertUserCourseAssignmentModel.userId,insertUserCourseAssignmentModel.courseId);
            if (userCourse != null)
            {
                _unitOfWork.UserCourseRepository.Remove(userCourse);
                _unitOfWork.Complete();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "UserCourse not found.");
            }
            return RedirectToAction();
        }

    }
}
