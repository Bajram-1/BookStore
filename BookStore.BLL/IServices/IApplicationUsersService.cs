using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IApplicationUsersService
    {
        Task<DTO.ApplicationUser> GetAsync(Expression<Func<DAL.Entities.ApplicationUser, bool>> filter);
        Task<RoleManagmentViewModel> GetUserRoleManagementAsync(string userId);
        Task<bool> ManageUserRoleAsync(RoleManagmentViewModel roleManagmentVM);
        Task<List<DAL.Entities.ApplicationUser>> GetAllUsersAsync();
        Task<bool> LockUnlockUserAsync(string userId);
    }
}
