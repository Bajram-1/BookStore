using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
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

        public void Add(ApplicationUser applicationUser)
        {
            _dbSet.Add(applicationUser);
        }

        public void Remove(ApplicationUser applicationUser)
        {
            _dbSet.Remove(applicationUser);
        }

        public void RemoveRange(IEnumerable<ApplicationUser> applicationUsers)
        {
            _dbSet.RemoveRange(applicationUsers);
        }

        public void Update(ApplicationUser applicationUser)
        {
            _dbSet.Update(applicationUser);
        }

        public ApplicationUser Get(Expression<Func<ApplicationUser, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<ApplicationUser> query;
            if (tracked)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter, string? includeProperties = null)
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
            return query.ToList();
        }

        public ApplicationUser GetById(string userId)
        {
            return _dbSet.Find(userId) ?? throw new Exception("User not found");
        }

        public ApplicationUser GetUserWithCompany(string userId)
        {
            return applicationDbContext.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefault(u => u.Id == userId);
        }
    }
}