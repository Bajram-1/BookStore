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
    public class ProductImagesRepository(ApplicationDbContext applicationDbContext) : IProductImagesRepository
    {
        private readonly DbSet<ProductImage> _dbSet = applicationDbContext.Set<ProductImage>();

        public void Update(ProductImage productImage)
        {
            _dbSet.Update(productImage);
        }

        public void Add(ProductImage productImage)
        {
            _dbSet.Add(productImage);
        }

        public ProductImage Get(Expression<Func<ProductImage, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<ProductImage> query;
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

        public IEnumerable<ProductImage> GetAll(Expression<Func<ProductImage, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<ProductImage> query = _dbSet;
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

        public void Remove(ProductImage productImage)
        {
            _dbSet.Remove(productImage);
        }

        public void RemoveRange(IEnumerable<ProductImage> productImages)
        {
            _dbSet.RemoveRange(productImages);
        }
    }
}