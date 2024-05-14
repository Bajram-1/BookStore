using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IProductsRepository : IBaseRepository<Product, int>
    {
        Task UpdateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null, string includeProperties = null);
        Task<Product> GetByISBNAsync(string isbn);
        Task<Product> GetByTitleAndAuthorAsync(string title, string author);
    }
}