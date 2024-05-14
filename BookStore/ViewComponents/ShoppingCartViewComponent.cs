using BookStore.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStore.DAL.IRepositories;
using BookStore.BLL.IServices;

namespace BookStore.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartsService _shoppingCartService;

        public ShoppingCartViewComponent(IShoppingCartsService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cartItemCount = await _shoppingCartService.GetCartItemCountAsync(claim.Value);
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, cartItemCount);

                return View(cartItemCount);
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
