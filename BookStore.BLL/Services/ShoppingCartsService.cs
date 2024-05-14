using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Linq.Expressions;

namespace BookStore.BLL.Services
{
    public class ShoppingCartsService(IShoppingCartsRepository shoppingCartsRepository,
                                      IUnitOfWork unitOfWork,
                                      IHttpContextAccessor httpContextAccessor) : IShoppingCartsService
    {
        public static event OnEntityAdded OnShoppingCartAdded;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await shoppingCartsRepository.GetCartItemCountAsync(userId);
        }

        public async Task<DAL.Entities.ShoppingCart> GetAsync(Expression<Func<BookStore.DAL.Entities.ShoppingCart, bool>> filter)
        {
            var dbCart = await shoppingCartsRepository.GetAsync(filter) ?? throw new Exception("Cart not found");
            return new DAL.Entities.ShoppingCart
            {
                Id = dbCart.Id,
                ApplicationUserId = dbCart.ApplicationUserId,
                ProductId = dbCart.ProductId,
                Count = dbCart.Count,
                Price = dbCart.Price
            };
        }

        public async Task DeleteAsync(int id)
        {
            var shoppingCartItem = await shoppingCartsRepository.GetAsync(s => s.Id == id);

            if (shoppingCartItem == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            await shoppingCartsRepository.DeleteAsync(shoppingCartItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(DTO.ShoppingCart shoppingCartDTO, string userId)
        {
            var cartFromDb = await shoppingCartsRepository.GetAsync(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCartDTO.ProductId,
                includeProperties: "Product"
            );

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCartDTO.Count;

                var productDescription = cartFromDb.Product.Description;
                var productTitle = cartFromDb.Product.Title;

                await shoppingCartsRepository.UpdateAsync(cartFromDb);
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                var shoppingCartEntity = new DAL.Entities.ShoppingCart
                {
                    ProductId = shoppingCartDTO.ProductId,
                    Count = shoppingCartDTO.Count,
                    Price = shoppingCartDTO.Price,
                    ApplicationUserId = userId,
                };

                await shoppingCartsRepository.CreateAsync(shoppingCartEntity);
                await unitOfWork.SaveChangesAsync();

                var shoppingCarts = await shoppingCartsRepository.GetAllAsync(u => u.ApplicationUserId == userId);
                var sessionCartCount = shoppingCarts.Count();
                httpContextAccessor.HttpContext.Session.SetInt32(StaticDetails.SessionCart, sessionCartCount);
            }
        }

        public async Task DecreaseItemCountAsync(int cartId)
        {
            var cartFromDb = await shoppingCartsRepository.GetAsync(u => u.Id == cartId);

            if (cartFromDb.Count <= 1)
            {
                await shoppingCartsRepository.DeleteAsync(cartFromDb);

                var shoppingCarts = await shoppingCartsRepository.GetAllAsync(u => u.ApplicationUserId == cartFromDb.ApplicationUserId);
                var newCartCount = shoppingCarts.Count() - 1;

                httpContextAccessor.HttpContext.Session.SetInt32(StaticDetails.SessionCart, newCartCount);
            }
            else
            {
                cartFromDb.Count -= 1;
                await shoppingCartsRepository.UpdateAsync(cartFromDb);
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddCartItemAsync(int cartId)
        {
            var cartFromDb = await shoppingCartsRepository.GetAsync(u => u.Id == cartId);
            if (cartFromDb == null)
            {
                throw new Exception("Shopping cart item not found");
            }

            cartFromDb.Count += 1;
            await shoppingCartsRepository.UpdateAsync(cartFromDb);
            await unitOfWork.SaveChangesAsync();
            OnShoppingCartAdded?.Invoke(new DAL.Entities.AuditLog
            {
                Details = Newtonsoft.Json.JsonConvert.SerializeObject(cartFromDb),
                EntityId = cartFromDb.Id.ToString(),
                EntityName = "ShoppingCarts",
                LogType = Common.Enums.AuditLogType.Create,
            });
        }

        public async Task<List<BLL.DTO.ShoppingCart>> GetAllAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter)
        {
            var dalShoppingCarts = await shoppingCartsRepository.GetAllAsync(filter);

            var dtoShoppingCarts = dalShoppingCarts.Select(cart => new BLL.DTO.ShoppingCart
            {
                Id = cart.Id,
                ApplicationUserId = cart.ApplicationUserId,
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Price
            }).ToList();

            return dtoShoppingCarts;
        }

        public async Task RemoveRangeAsync(List<BLL.DTO.ShoppingCart> shoppingCarts)
        {
            var dalShoppingCarts = shoppingCarts.Select(cart => new DAL.Entities.ShoppingCart
            {
                Id = cart.Id,
                ApplicationUserId = cart.ApplicationUserId,
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Price
            }).ToList();

            await shoppingCartsRepository.RemoveRangeAsync(dalShoppingCarts);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<BLL.DTO.ShoppingCart>> GetShoppingCartsByUserIdAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> predicate, string includeProperties = "Product")
        {
            try
            {
                var shoppingCarts = await shoppingCartsRepository.GetAllAsync(predicate, includeProperties: "Product") ?? Enumerable.Empty<DAL.Entities.ShoppingCart>();

#pragma warning disable CS8601
                var shoppingCartDTOs = shoppingCarts.Select(cart => new BLL.DTO.ShoppingCart
                {
                    Id = cart.Id,
                    ApplicationUserId = cart.ApplicationUserId,
                    ProductId = cart.Product.Id,
                    Count = cart.Count,
                    Price = cart.Product.Price,
                    Product = cart.Product != null ? new BLL.DTO.Product
                    {
                        Id = cart.Product.Id,
                        Title = cart.Product.Title,
                        Author = cart.Product.Author,
                        ListPrice = cart.Product.ListPrice,
                        Price = cart.Product.Price,
                        Price50 = cart.Product.Price50,
                        Price100 = cart.Product.Price100,
                        Description = cart.Product.Description,
                        ISBN = cart.Product.ISBN,
                        CategoryId = cart.Product.CategoryId,
                        ProductImages = cart.Product.ProductImages?.Select(img => new DTO.ProductImage
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl
                        }).ToList() ?? new List<DTO.ProductImage>()
                    } : null
                }).ToList();
#pragma warning restore CS8601

                return shoppingCartDTOs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve shopping carts", ex);
            }
        }
    }
}