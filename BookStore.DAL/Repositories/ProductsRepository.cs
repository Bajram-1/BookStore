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
    public class ProductsRepository(ApplicationDbContext applicationDbContext) : IProductsRepository
    {
        private readonly DbSet<Product> _dbSet = applicationDbContext.Set<Product>();

        public void Add(Product product)
        {
            _dbSet.Add(product);
        }

        public Product Get(Expression<Func<Product, bool>> filter, string includeProperties = null, bool tracked = false)
        {
            IQueryable<Product> query = _dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.FirstOrDefault(filter);
        }

        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<Product> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }

        public void Remove(Product product)
        {
            _dbSet.Remove(product);
        }

        public void RemoveRange(IEnumerable<Product> products)
        {
            _dbSet.RemoveRange(products);
        }

        public void Update(Product product)
        {
            _dbSet.Update(product);
        }
    }
}