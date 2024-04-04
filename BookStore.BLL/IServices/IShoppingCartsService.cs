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
        int GetCartItemCount(string userId);
        DTO.ShoppingCart Create(ShoppingCartAddEditRequestModel model);
        void Delete(int id);
        IEnumerable<DTO.ShoppingCart> GetCartItems(string userId);
        void Update(DTO.ShoppingCart model);
        void RemoveRange(IEnumerable<DTO.ShoppingCart> cartItems);
        IEnumerable<DTO.ShoppingCart> GetAll(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter = null, string includeProperties = null);
        DTO.ShoppingCart Get(Expression<Func<BookStore.DAL.Entities.ShoppingCart, bool>> filter);
        void UpdateCart(DAL.Entities.ShoppingCart shoppingCart, string userId);
        void ProcessOrder(string userId, ShoppingCartVM shoppingCartVM);
        void DecreaseItemCount(int cartId);
        void AddCartItem(int cartId);
        int DeleteItem(int id);
        void ProcessOrderConfirmation(int orderId);
        double GetPriceBasedOnQuantity(BLL.DTO.ShoppingCart shoppingCart);
    }
}
