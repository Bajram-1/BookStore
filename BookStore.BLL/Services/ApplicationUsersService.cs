using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BookStore.BLL.DTO.Requests;
using System.Data;
using System.Web.Helpers;
using BookStore.BLL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookStore.BLL.Services
{
    public class ApplicationUsersService(IApplicationUsersRepository applicationUsersRepository, ICompaniesRepository companiesRepository,
                                         UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
                                         IUnitOfWork unitOfWork) : IApplicationUsersService
    {
        public async Task<DTO.ApplicationUser> GetAsync(Expression<Func<DAL.Entities.ApplicationUser, bool>> filter)
        {
            var dbUser = await applicationUsersRepository.GetAsync(filter) ?? throw new Exception("User not found");
            return new DTO.ApplicationUser
            {
                Id = dbUser.Id,
                CompanyId = dbUser.CompanyId,
                Email = dbUser.Email,
                Name = dbUser.Name,
                PhoneNumber = dbUser.PhoneNumber,
                Role = dbUser.Role,
                StreetAddress = dbUser.StreetAddress,
                City = dbUser.City,
                State = dbUser.State,
                PostalCode = dbUser.PostalCode
            };
        }

        public async Task<RoleManagmentViewModel> GetUserRoleManagementAsync(string userId)
        {
            var userDAL = await applicationUsersRepository.GetUserWithCompanyAsync(userId);

            var userBLL = new BLL.DTO.ApplicationUser
            {
                Id = userDAL.Id,
                UserName = userDAL.UserName,
                Email = userDAL.Email,
                CompanyId = userDAL.CompanyId,
                City = userDAL.City,
                Name = userDAL.Name,
                PhoneNumber = userDAL.PhoneNumber,
                PostalCode = userDAL.PostalCode,
                Role = userDAL.Role,
                State = userDAL.State,
                StreetAddress = userDAL.StreetAddress
            };

            var roleVM = new RoleManagmentViewModel
            {
                ApplicationUser = userBLL,
                RoleList = roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }).ToList(),
                CompanyList = (await companiesRepository.GetAllAsync()).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }).ToList(),
            };

            var userRoles = await userManager.GetRolesAsync(userDAL);
            roleVM.ApplicationUser.Role = userRoles.FirstOrDefault();

            return roleVM;
        }

        public async Task<bool> ManageUserRoleAsync(RoleManagmentViewModel roleManagmentVM)
        {
            var applicationUser = await applicationUsersRepository.GetByIdAsync(roleManagmentVM.ApplicationUser.Id);
            var oldRole = (await userManager.GetRolesAsync(applicationUser)).FirstOrDefault();

            if (roleManagmentVM.ApplicationUser.Role != oldRole)
            {
                if (roleManagmentVM.ApplicationUser.Role == StaticDetails.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                else if (oldRole == StaticDetails.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                await userManager.RemoveFromRoleAsync(applicationUser, oldRole);
                await userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role);
            }
            else
            {
                if (oldRole == StaticDetails.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
            }

            await applicationUsersRepository.UpdateAsync(applicationUser);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<DAL.Entities.ApplicationUser>> GetAllUsersAsync()
        {
            var users = await unitOfWork.ApplicationUserRepository.GetAllAsync(includeProperties: "Company");

            foreach (var user in users)
            {
                user.Role = (await userManager.GetRolesAsync(user)).FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new DAL.Entities.Company()
                    {
                        Name = ""
                    };
                }
            }

            return users.ToList();
        }

        public async Task<bool> LockUnlockUserAsync(string userId)
        {
            var user = await unitOfWork.ApplicationUserRepository.GetAsync(u => u.Id == userId);

            if (user == null)
                return false;

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            await unitOfWork.ApplicationUserRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}