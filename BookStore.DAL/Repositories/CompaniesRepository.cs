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
    public class CompaniesRepository(ApplicationDbContext applicationDbContext) : BaseRepository<Company, int>(applicationDbContext), ICompaniesRepository
    {
        public async Task<Company> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            var company = await _dbSet.FindAsync(id);
            if (company == null)
            {
                throw new Exception("Company not found");
            }
            return company;
        }

        public async Task<IEnumerable<Company>> GetAllAsync(Expression<Func<Company, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<Company> query = _dbSet;

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
    }
}