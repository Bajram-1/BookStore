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
    public class ProductsRepository(ApplicationDbContext applicationDbContext) : BaseRepository<Product, int>(applicationDbContext), IProductsRepository
    {
        public async Task UpdateAsync(Product product)
        {
            var local = _dbSet.Local.FirstOrDefault(entry => entry.Id.Equals(product.Id));

            if (local != null)
            {
                _dbSet.Entry(local).State = EntityState.Detached;
            }

            _dbSet.Update(product);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null, string includeProperties = null)
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

            IEnumerable<Product> productsDTO = await query.Select(entity => new Product
            {
                Id = entity.Id,
                Author = entity.Author,
                Category = entity.Category,
                CategoryId = entity.CategoryId,
                Description = entity.Description,
                ISBN = entity.ISBN,
                ListPrice = entity.ListPrice,
                Price = entity.Price,
                Price50 = entity.Price50,
                Price100 = entity.Price100,
                ProductImages = entity.ProductImages,
                Title = entity.Title,
            }).ToListAsync();

            return productsDTO;
        }

        public async Task<Product> GetByISBNAsync(string isbn)
        {
            return await _dbSet.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ISBN == isbn);
        }

        public async Task<Product> GetByTitleAndAuthorAsync(string title, string author)
        {
            return await _dbSet.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Title == title && p.Author == author);
        }
    }
}