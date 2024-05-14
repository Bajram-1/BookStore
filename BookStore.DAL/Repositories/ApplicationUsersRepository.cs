using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class ApplicationUsersRepository(ApplicationDbContext applicationDbContext) : IApplicationUsersRepository
    {
        private readonly DbSet<ApplicationUser> _dbSet = applicationDbContext.Set<ApplicationUser>();

        public async Task UpdateAsync(ApplicationUser applicationUser)
        {
            _dbSet.Update(applicationUser);
            await Task.CompletedTask;
        }

        public async Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> filter, string includeProperties = null, bool tracked = false)
        {
            IQueryable<ApplicationUser> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(Expression<Func<ApplicationUser, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<ApplicationUser> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<ApplicationUser> GetUserWithCompanyAsync(string userId)
        {
            return await applicationDbContext.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}