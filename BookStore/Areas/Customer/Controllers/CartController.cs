using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using BookStore.Models;
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
                              IOrderHeadersService orderHeadersService,
                              IOrderDetailsService orderDetailsService,
                              IEmailSender emailSender,
                              IHttpContextAccessor httpContextAccessor) : Controller
    {
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var productImagesDTO = await productImagesService.GetProductImagesAsync();

            ShoppingCartVM = new()
            {
                ShoppingCartList = await shoppingCartsService.GetShoppingCartsByUserIdAsync(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new BLL.DTO.OrderHeader()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                if (cart.Product != null)
                {
                    var productImages = productImagesDTO.Select(pi => new BLL.DTO.ProductImage
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageUrl,
                        ProductId = pi.ProductId,
                    }).ToList();

                    cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
                }
            }

            return View(ShoppingCartVM);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = await shoppingCartsService.GetShoppingCartsByUserIdAsync(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = await applicationUsersService.GetAsync(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = await shoppingCartsService.GetShoppingCartsByUserIdAsync(u => u.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            BLL.DTO.ApplicationUser applicationUser = await applicationUsersService.GetAsync(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }

            var dtoOrderHeader = await orderHeadersService.AddAsync(ShoppingCartVM.OrderHeader);
            var orderHeaderId = dtoOrderHeader.Id;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                BLL.DTO.OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = orderHeaderId,
                    Price = GetPriceBasedOnQuantity(cart),
                    Count = cart.Count
                };
                await orderDetailsService.AddOrderDetailAsync(orderDetail, orderHeaderId);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={orderHeaderId}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                await orderHeadersService.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeaderId });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader orderHeader = await orderHeadersService.GetAsync(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await orderHeadersService.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    await orderHeadersService.UpdateStatusAsync(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                }
                HttpContext.Session.Clear();
            }

            await emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Book Store",
              $"<p>New Order Created - {orderHeader.Id}</p>");

            IEnumerable<ShoppingCart> shoppingCartsEnumerable = await shoppingCartsService.GetAllAsync(u => u.ApplicationUserId == orderHeader.ApplicationUserId);
            List<ShoppingCart> shoppingCarts = shoppingCartsEnumerable.ToList();

            await shoppingCartsService.RemoveRangeAsync(shoppingCarts);

            return View(id);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            await shoppingCartsService.AddCartItemAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            await shoppingCartsService.DecreaseItemCountAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await shoppingCartsService.GetAsync(u => u.Id == cartId);

            if (cartFromDb == null)
            {
                return NotFound();
            }

            await shoppingCartsService.DeleteAsync(cartId);

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var remainingCount = await shoppingCartsService.GetCartItemCountAsync(userId);
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, remainingCount - 1);
            }

            return RedirectToAction(nameof(Index));
        }

        public decimal GetPriceBasedOnQuantity(BLL.DTO.ShoppingCart shoppingCart)
        {
            return shoppingCart.Count <= 50 ? shoppingCart.Product.Price :
                    shoppingCart.Count <= 100 ? shoppingCart.Product.Price50 :
                    shoppingCart.Product.Price100;
        }
    }
}