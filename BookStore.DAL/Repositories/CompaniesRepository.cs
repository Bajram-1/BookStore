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
    public class CompaniesRepository(ApplicationDbContext applicationDbContext) : ICompaniesRepository
    {
        private readonly DbSet<Company> _dbSet = applicationDbContext.Set<Company>();

        public void Update(Company company)
        {
            _dbSet.Update(company);
        }

        public void Add(Company company)
        {
            _dbSet.Add(company);
        }

        public Company Get(Expression<Func<Company, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<Company> query;
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

        public Company? GetByName(string name)
        {
            return _dbSet.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<Company> GetAll(Expression<Func<Company, bool>>? filter, string? includeProperties = null)
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
            return query.ToList();
        }

        public void Remove(Company company)
        {
            _dbSet.Remove(company);
        }

        public void RemoveRange(IEnumerable<Company> companies)
        {
            _dbSet.RemoveRange(companies);
        }

        public Company GetById(int id)
        {
            return _dbSet.Find(id) ?? throw new Exception("Company not found");
        }
    }
}