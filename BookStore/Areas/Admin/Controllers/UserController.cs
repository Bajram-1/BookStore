using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
                                IUnitOfWork unitOfWork, IApplicationUsersService applicationUsersService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoleManagment(string userId)
        {
            var roleVM = applicationUsersService.GetUserRoleManagement(userId);
            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            try
            {
                applicationUsersService.ManageUserRole(roleManagmentVM);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred.";
            }

            return View(roleManagmentVM);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<DAL.Entities.ApplicationUser> objUserList = unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in objUserList)
            {

                user.Role = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new DAL.Entities.Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {

            var objFromDb = unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            unitOfWork.ApplicationUser.Update(objFromDb);
            unitOfWork.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
