using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IShoppingCartsRepository
    {
        void Update(ShoppingCart cart);
        IEnumerable<ShoppingCart> GetAll(Expression<Func<ShoppingCart, bool>>? filter = null, string? includeProperties = null);
        ShoppingCart Get(Expression<Func<ShoppingCart, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(ShoppingCart shoppingCart);
        void Remove(ShoppingCart shoppingCart);
        void RemoveRange(IEnumerable<ShoppingCart> shoppingCarts);
        IEnumerable<ShoppingCart> GetCartItems(string userId);
    }
}