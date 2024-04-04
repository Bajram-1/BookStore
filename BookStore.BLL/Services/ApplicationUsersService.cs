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
        public void AddApplicationUser(ApplicationUserAddEditRequestModel userObj)
        {
            var user = new DAL.Entities.ApplicationUser
            {
                Id = userObj.Id,
                City = userObj.City,
                UserName = userObj.Name,
                CompanyId = userObj.CompanyId,
                PostalCode = userObj.PostalCode,
                State = userObj.State,
                Role = userObj.Role,
                StreetAddress = userObj.StreetAddress,
                PhoneNumber = userObj.PhoneNumber,
                Email = userObj.Email
            };

            applicationUsersRepository.Add(user);
            unitOfWork.SaveChanges();
        }

        public void RemoveApplicationUser(string userId)
        {
            var applicationUser = applicationUsersRepository.Get(u => u.Id == userId);

            if (applicationUser != null)
            {
                applicationUsersRepository.Remove(applicationUser);
                unitOfWork.SaveChanges();
            }
        }

        public void UpdateApplicationUser(string id, ApplicationUserAddEditRequestModel userObj)
        {
            var user = applicationUsersRepository.GetById(id);

            user.Id = userObj.Id;
            user.City = userObj.City;
            user.UserName = userObj.Name;
            user.CompanyId = userObj.CompanyId;
            user.PostalCode = userObj.PostalCode;
            user.State = userObj.State;
            user.Role = userObj.Role;
            user.StreetAddress = userObj.StreetAddress;
            user.PhoneNumber = userObj.PhoneNumber;
            user.Email = userObj.Email;

            unitOfWork.SaveChanges();
        }

        public IEnumerable<DAL.Entities.ApplicationUser> GetAllApplicationUsers(Expression<Func<DAL.Entities.ApplicationUser, bool>>? filter = null, string? includeProperties = null)
        {
            return applicationUsersRepository.GetAll(filter, includeProperties);
        }

        public DTO.ApplicationUser GetApplicationUserById(string userId)
        {
            var dbUser = applicationUsersRepository.GetById(userId) ?? throw new Exception("User not found");
            return new DTO.ApplicationUser
            {

                Id = dbUser.Id,
                City = dbUser.City,
                Name = dbUser.Name,
                CompanyId = dbUser.CompanyId,
                PostalCode = dbUser.PostalCode,
                State = dbUser.State,
                StreetAddress = dbUser.StreetAddress,
                PhoneNumber = dbUser.PhoneNumber,
            };
        }

        public void RemoveRange(IEnumerable<DAL.Entities.ApplicationUser> applicationUsers)
        {
            applicationUsersRepository.RemoveRange(applicationUsers);
            unitOfWork.SaveChanges();
        }

        public DTO.ApplicationUser Get(Expression<Func<DAL.Entities.ApplicationUser, bool>> filter)
        {
            var dbUser = applicationUsersRepository.Get(filter) ?? throw new Exception("User not found");
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

        public RoleManagmentVM GetUserRoleManagement(string userId)
        {
            var user = applicationUsersRepository.GetUserWithCompany(userId);

            var roleVM = new RoleManagmentVM
            {
                ApplicationUser = user,
                RoleList = roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = companiesRepository.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            var userRole = userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            roleVM.ApplicationUser.Role = userRole.FirstOrDefault();

            return roleVM;
        }

        public bool ManageUserRole(RoleManagmentVM roleManagmentVM)
        {
            var applicationUser = applicationUsersRepository.GetById(roleManagmentVM.ApplicationUser.Id);
            var oldRole = userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult().FirstOrDefault();

            if (roleManagmentVM.ApplicationUser.Role != oldRole)
            {
                // A role was updated
                if (roleManagmentVM.ApplicationUser.Role == StaticDetails.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                else if (oldRole == StaticDetails.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                // No role change, but check for company change if applicable
                if (oldRole == StaticDetails.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
            }

            applicationUsersRepository.Update(applicationUser);
            return true;
        }

        public DTO.ApplicationUser GetApplicationUser(string userId)
        {
            var user = applicationUsersRepository.Get(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var users = new DTO.ApplicationUser
            {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode
            };

            return users;
        }
    }
}