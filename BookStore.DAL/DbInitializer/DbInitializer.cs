using BookStore.Common;
using BookStore.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public async Task InitializeAsync()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch
            {
                throw;
            }

            if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_Customer))
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer));
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company));

                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Bajram Shehi",
                    PhoneNumber = "0695668776",
                    StreetAddress = "Rruga Arben Lami",
                    State = "Shqiperi",
                    PostalCode = "1001",
                    City = "Tirana"
                }, "Albania128.");

                ApplicationUser user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
                await _userManager.AddToRoleAsync(user, StaticDetails.Role_Admin);
            }
        }
    }
}
