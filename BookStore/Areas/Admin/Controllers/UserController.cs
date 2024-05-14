using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
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
    public class UserController(IApplicationUsersService applicationUsersService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> RoleManagment(string userId)
        {
            var roleVM = await applicationUsersService.GetUserRoleManagementAsync(userId);
            return View(roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentViewModel roleManagmentVM)
        {
            try
            {
                await applicationUsersService.ManageUserRoleAsync(roleManagmentVM);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred: " + ex.Message;
                return View(roleManagmentVM);
            }
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await applicationUsersService.GetAllUsersAsync();
            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string userId)
        {
            bool success = await applicationUsersService.LockUnlockUserAsync(userId);

            if (success)
            {
                return Json(new { success = true, message = "Operation Successful" });
            }
            else
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
        }

        #endregion
    }
}
