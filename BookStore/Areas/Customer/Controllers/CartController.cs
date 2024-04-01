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
            try
            {
                var userId = GetCurrentUserId();
                var shoppingCartList = shoppingCartsService.GetCartItems(userId);
                var orderTotal = CalculateOrderTotal(shoppingCartList);
                PopulateProductImages(shoppingCartList);

                var shoppingCartVM = new ShoppingCartVM
                {
                    ShoppingCartList = shoppingCartList,
                    OrderHeader = new OrderHeaderAddEditRequestModel { OrderTotal = orderTotal }
                };

                return View(shoppingCartVM);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public IActionResult Summary()
        {
            var userId = GetCurrentUserId();
            var shoppingCartList = shoppingCartsService.GetCartItems(userId);
            var orderTotal = CalculateOrderTotal(shoppingCartList);
            var user = applicationUsersService.GetApplicationUser(userId);

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = shoppingCartList,
                OrderHeader = new OrderHeaderAddEditRequestModel { OrderTotal = orderTotal }
            };

            if (user != null)
            {
                shoppingCartVM.OrderHeader.Name = user.Name;
                shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
                shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
                shoppingCartVM.OrderHeader.City = user.City;
                shoppingCartVM.OrderHeader.State = user.State;
                shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var userId = GetCurrentUserId();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        private string GetCurrentUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private double CalculateOrderTotal(IEnumerable<BLL.DTO.ShoppingCart> shoppingCartList)
        {
            double orderTotal = 0.0;
            foreach (var cart in shoppingCartList)
            {
                if (cart.Product != null)
                {
                    cart.Price = shoppingCartsService.GetPriceBasedOnQuantity(cart);
                    orderTotal += (cart.Price * cart.Count);
                }
                else
                {
                    Console.WriteLine($"Warning: Product is null for ShoppingCart with ID {cart.Id}. Skipping item.");
                }
            }
            return orderTotal;
        }
        private void PopulateProductImages(IEnumerable<BLL.DTO.ShoppingCart> shoppingCartList)
        {
            foreach (var cart in shoppingCartList)
            {
                if (cart.Product != null)
                {
                    var productImages = productImagesService.GetAllProductImages()
                        .Where(u => u.ProductId != null && u.ProductId == cart.Product.Id)
                        .ToList();

                    cart.Product.ProductImages = productImages;
                }
                else
                {
                    Console.WriteLine($"Warning: Product is null for ShoppingCart with ID {cart.Id}. Unable to populate images.");
                }
            }
        }
    }
}