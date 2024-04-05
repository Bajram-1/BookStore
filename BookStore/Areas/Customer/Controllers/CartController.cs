using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController(IShoppingCartsService shoppingCartsService,
                              IProductImagesService productImagesService,
                              IApplicationUsersService applicationUsersService,
                              IHttpContextAccessor httpContextAccessor) : Controller
    {
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var shoppingCartList = shoppingCartsService.GetCartItems(userId);

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = shoppingCartList,
                OrderHeader = new OrderHeaderAddEditRequestModel()
            };

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                if (cart.Product != null)
                {
                    var productImages = productImagesService.GetProductImagesByProductId(cart.Product.Id);
                    if (productImages != null)
                    {
                        cart.Product.ProductImages = productImages.ToList();
                    }
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }
            }

            return View(shoppingCartVM);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var shoppingCartList = shoppingCartsService.GetCartItems(userId);

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = shoppingCartList,
                OrderHeader = new OrderHeaderAddEditRequestModel()
            };

            var user = applicationUsersService.GetApplicationUserById(userId);

            if (user != null)
            {
                shoppingCartVM.OrderHeader.ApplicationUser = user;
                shoppingCartVM.OrderHeader.Name = user.Name;
                shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
                shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
                shoppingCartVM.OrderHeader.City = user.City;
                shoppingCartVM.OrderHeader.State = user.State;
                shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            }

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var shoppingCartVM = new ShoppingCartVM();
            shoppingCartsService.ProcessOrder(userId, shoppingCartVM);
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
        }

        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {
            try
            {
                shoppingCartsService.ProcessOrderConfirmation(id);
                return View(id);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while processing the order confirmation.";
                return View("Error");
            }
        }

        public IActionResult Plus(int cartId)
        {
            shoppingCartsService.AddCartItem(cartId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            shoppingCartsService.DecreaseItemCount(cartId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = shoppingCartsService.Get(u => u.Id == cartId);

            if (cartFromDb == null)
            {
                return NotFound();
            }

            shoppingCartsService.Delete(cartId);

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var remainingCount = shoppingCartsService.GetCartItemCount(userId);
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, remainingCount - 1);
            }

            return RedirectToAction(nameof(Index));
        }

        public double GetPriceBasedOnQuantity(BLL.DTO.ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}