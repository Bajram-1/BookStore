using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BookStore.BLL.Services
{
    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IOrderHeadersService _orderHeadersService;
        private readonly IOrderDetailsService _orderDetailsService;
        private readonly IOrderHeadersRepository _orderHeadersRepository;
        private readonly IShoppingCartsRepository _shoppingCartsRepository;
        private readonly IProductsService _productsService;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartsService(IApplicationUsersService applicationUsersService,
                                    IOrderHeadersService orderHeadersService,
                                    IOrderDetailsService orderDetailsService,
                                    IProductsService productsService,
                                    IOrderHeadersRepository orderHeadersRepository,
                                    IShoppingCartsRepository shoppingCartsRepository,
                                    IEmailSender emailSender,
                                    IUnitOfWork unitOfWork,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _applicationUsersService = applicationUsersService;
            _orderHeadersService = orderHeadersService;
            _orderDetailsService = orderDetailsService;
            _productsService = productsService;
            _orderHeadersRepository = orderHeadersRepository;
            _shoppingCartsRepository = shoppingCartsRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCartItemCount(string userId)
        {
            return _shoppingCartsRepository.GetCartItems(userId).Sum(item => item.Count);
        }

        public DTO.ShoppingCart Get(Expression<Func<BookStore.DAL.Entities.ShoppingCart, bool>> filter)
        {
            var dbCart = _shoppingCartsRepository.Get(filter) ?? throw new Exception("Cart not found");
            return new DTO.ShoppingCart
            {
                Id = dbCart.Id,
                ApplicationUserId = dbCart.ApplicationUserId,
                ProductId = dbCart.ProductId,
                Count = dbCart.Count,
                Price = dbCart.Price
            };
        }

        public DTO.ShoppingCart Create(ShoppingCartAddEditRequestModel model)
        {
            var shoppingCart = new DAL.Entities.ShoppingCart
            {
                Id = model.Id,
                ApplicationUserId = model.ApplicationUserId,
                ProductId = model.ProductId,
                Count = model.Count,
                Price = model.Price
            };

            var productWithPrice = _productsService.GetProductById(model.ProductId);
            if (productWithPrice == null)
            {
                throw new Exception("Product not found");
            }

            shoppingCart.Price = productWithPrice.Price * model.Count;

            _shoppingCartsRepository.Add(shoppingCart);
            _unitOfWork.SaveChanges();

            return GetById(shoppingCart.Id);
        }


        private DTO.ShoppingCart GetById(int id)
        {
            var dbShoppingCart = _shoppingCartsRepository.Get(s => s.Id == id);

            if (dbShoppingCart == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            return new DTO.ShoppingCart
            {
                Id = dbShoppingCart.Id,
                ApplicationUserId = dbShoppingCart.ApplicationUserId,
                ProductId = dbShoppingCart.ProductId,
                Count = dbShoppingCart.Count
            };
        }

        public void Delete(int id)
        {
            var shoppingCartItem = _shoppingCartsRepository.Get(s => s.Id == id);

            if (shoppingCartItem == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            _shoppingCartsRepository.Remove(shoppingCartItem);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.ShoppingCart> GetCartItems(string userId)
        {
            var dbShoppingCartItems = _shoppingCartsRepository.GetCartItems(userId);

            var result = new List<DTO.ShoppingCart>();
            foreach (var shoppingCartItem in dbShoppingCartItems)
            {

                result.Add(new DTO.ShoppingCart
                {
                    Id = shoppingCartItem.Id,
                    ApplicationUserId = shoppingCartItem.ApplicationUserId,
                    ProductId = shoppingCartItem.ProductId,
                    Count = shoppingCartItem.Count,
                    Price = shoppingCartItem.Product.Price
            });
            }
            return result;
        }

        public void Update(DTO.ShoppingCart model)
        {
            var shoppingCartItemToUpdate = _shoppingCartsRepository.Get(s => s.Id == model.Id);
            if (shoppingCartItemToUpdate == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            shoppingCartItemToUpdate.Count = model.Count;

            _unitOfWork.SaveChanges();
        }

        public void RemoveRange(IEnumerable<DTO.ShoppingCart> cartItems)
        {
            var shoppingCartItems = _shoppingCartsRepository.GetAll().ToList();

            if (shoppingCartItems == null)
            {
                throw new Exception("Shopping cart items not found");
            }

            _shoppingCartsRepository.RemoveRange(shoppingCartItems);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.ShoppingCart> GetAll(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<DAL.Entities.ShoppingCart> query = _shoppingCartsRepository.GetAll().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                if (includeProperties == "Product")
                {
                    query = query.Include(u => u.Product);
                }
            }

            return query.Select(shoppingCart => new DTO.ShoppingCart
            {
                Id = shoppingCart.Id,
                ApplicationUserId = shoppingCart.ApplicationUserId,
                ProductId = shoppingCart.ProductId,
                Count = shoppingCart.Count
            }).ToList();
        }

        public void UpdateCart(DAL.Entities.ShoppingCart shoppingCart, string userId)
        {
            shoppingCart.ApplicationUserId = userId;

            var cartFromDb = _shoppingCartsRepository.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _shoppingCartsRepository.Update(cartFromDb);
                _unitOfWork.SaveChanges();
            }
            else
            {
                _shoppingCartsRepository.Add(shoppingCart);
                _unitOfWork.SaveChanges();

                var sessionCartCount = _shoppingCartsRepository.GetAll(u => u.ApplicationUserId == userId).Count();
                _httpContextAccessor.HttpContext.Session.SetInt32(StaticDetails.SessionCart, sessionCartCount);
            }
        }

        public void ProcessOrder(string userId, ShoppingCartVM shoppingCartVM)
        {
            shoppingCartVM.ShoppingCartList = GetCartItems(userId);
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            var applicationUser = _applicationUsersService.Get(u => u.Id == userId);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
                shoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }

            _orderHeadersService.Add(shoppingCartVM.OrderHeader);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                var orderDetail = new DAL.Entities.OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _orderDetailsService.AddOrderDetail(orderDetail);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
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
                _orderHeadersService.UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            }

            _unitOfWork.SaveChanges();
        }

        public void DecreaseItemCount(int cartId)
        {
            var cartFromDb = _shoppingCartsRepository.Get(u => u.Id == cartId);

            if (cartFromDb.Count <= 1)
            {
                _shoppingCartsRepository.Remove(cartFromDb);

                _httpContextAccessor.HttpContext.Session.SetInt32(StaticDetails.SessionCart, _shoppingCartsRepository
                    .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            }
            else
            {
                cartFromDb.Count -= 1;
                _shoppingCartsRepository.Update(cartFromDb);
            }

            _unitOfWork.SaveChanges();
        }

        public void AddCartItem(int cartId)
        {
            var cartFromDb = _shoppingCartsRepository.Get(u => u.Id == cartId);
            if (cartFromDb == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            cartFromDb.Count += 1;
            _shoppingCartsRepository.Update(cartFromDb);
            _unitOfWork.SaveChanges();
        }

        public int DeleteItem(int id)
        {
            var shoppingCartItem = _shoppingCartsRepository.Get(s => s.Id == id);

            if (shoppingCartItem == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            _shoppingCartsRepository.Remove(shoppingCartItem);
            _unitOfWork.SaveChanges();

            return GetAll().Count();
        }

        public void ProcessOrderConfirmation(int orderId)
        {
            var orderHeader = _orderHeadersRepository.GetItem(orderId, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.StripePaymentId = session.Id;
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    orderHeader.Status = StaticDetails.StatusApproved;
                    orderHeader.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    _unitOfWork.SaveChanges();
                }

                _httpContextAccessor.HttpContext.Session.Clear();
            }

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
                $"<p>New Order Created - {orderHeader.Id}</p>");

            var shoppingCarts = _shoppingCartsRepository.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            foreach (var shoppingCart in shoppingCarts)
            {
                _shoppingCartsRepository.Remove(shoppingCart);
            }
            _unitOfWork.SaveChanges();
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