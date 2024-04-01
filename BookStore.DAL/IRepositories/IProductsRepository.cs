using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IProductsRepository
    {
        void Update(Product product);
        IEnumerable<Product> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null);
        Product Get(Expression<Func<Product, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(Product product);
        void Remove(Product product);
        void RemoveRange(IEnumerable<Product> products);
    }
}