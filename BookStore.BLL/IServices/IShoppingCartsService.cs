using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IShoppingCartsService
    {
        Task<int> GetCartItemCountAsync(string userId);
        Task DeleteAsync(int id);
        Task<DAL.Entities.ShoppingCart> GetAsync(Expression<Func<BookStore.DAL.Entities.ShoppingCart, bool>> filter);
        Task UpdateCartAsync(DTO.ShoppingCart shoppingCartDTO, string userId);
        Task DecreaseItemCountAsync(int cartId);
        Task<List<BLL.DTO.ShoppingCart>> GetAllAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter);
        Task RemoveRangeAsync(List<BLL.DTO.ShoppingCart> shoppingCarts);
        Task AddCartItemAsync(int cartId);
        Task<IEnumerable<BLL.DTO.ShoppingCart>> GetShoppingCartsByUserIdAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> predicate, string includeProperties = "Product");
    }
}
