using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
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
        void AddApplicationUser(ApplicationUserAddEditRequestModel userObj);
        void RemoveApplicationUser(string userId);
        void UpdateApplicationUser(string id, ApplicationUserAddEditRequestModel userObj);
        IEnumerable<DAL.Entities.ApplicationUser> GetAllApplicationUsers(Expression<Func<DAL.Entities.ApplicationUser, bool>>? filter = null, string? includeProperties = null);
        ApplicationUserAddEditRequestModel GetApplicationUserById(string userId);
        void RemoveRange(IEnumerable<DAL.Entities.ApplicationUser> applicationUsers);
        DTO.ApplicationUser Get(Expression<Func<DAL.Entities.ApplicationUser, bool>> filter);
        RoleManagmentVM GetUserRoleManagement(string userId);
        bool ManageUserRole(RoleManagmentVM roleManagmentVM);
        DTO.ApplicationUser GetApplicationUser(string userId);
    }
}
