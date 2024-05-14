using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IShoppingCartsRepository : IBaseRepository<ShoppingCart, int>
    {
        Task UpdateAsync(ShoppingCart cart);
        Task<int> GetCartItemCountAsync(string userId);
        Task<List<DAL.Entities.ShoppingCart>> GetAllAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter);
    }
}