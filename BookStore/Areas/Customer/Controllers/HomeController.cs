using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.BLL.Services.Singletons;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

[Area("Customer")]
public class HomeController(IProductsService productsService,
                            IShoppingCartsService shoppingCartsService,
                            ILoggerService loggerService) : Controller
{
    public async Task<IActionResult> Index(int? categoryId = null)
    {
        var categories = await productsService.GetAllCategoriesAsync();
        ViewBag.Categories = categories.Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Id.ToString()
        }).ToList();

        ViewBag.SelectedCategory = categoryId;

        ViewBag.HasCategories = ((IEnumerable<SelectListItem>)ViewBag.Categories).Any();

        var productDTOsQuery = await productsService.GetAllCategoryProductImagesAsync(includeProperties: "Category,ProductImages");

        if (categoryId.HasValue)
        {
            productDTOsQuery = productDTOsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        var productDTOs = productDTOsQuery.Select(p => new BookStore.BLL.DTO.Product
        {
            Id = p.Id,
            Title = p.Title,
            Author = p.Author,
            ListPrice = p.ListPrice,
            Price = p.Price,
            Price50 = p.Price50,
            Price100 = p.Price100,
            Description = p.Description,
            ISBN = p.ISBN,
            CategoryId = p.CategoryId,
            Category = new BookStore.BLL.DTO.Category
            {
                Id = p.Category.Id,
                Name = p.Category.Name,
                Description = p.Category.Description
            },
            ProductImages = p.ProductImages.Select(pi => new BookStore.BLL.DTO.ProductImage
            {
                Id = pi.Id,
                ImageUrl = pi.ImageUrl,
                ProductId = pi.ProductId,
            }).ToList()
        }).ToList();

        return View(productDTOs);
    }

    public async Task<IActionResult> Details(int productId)
    {
        var productEntity = await productsService.GetProductDetailsAsync(productId, includeProperties: "Category,ProductImages");
        loggerService.Log("Info", $"Home page categories: {JsonConvert.SerializeObject(productEntity)}");

        BookStore.BLL.DTO.ShoppingCart cartItemDTO = new()
        {
            Product = new BookStore.BLL.DTO.Product
            {
                Id = productEntity.Id,
                Title = productEntity.Title,
                Author = productEntity.Author,
                ListPrice = productEntity.ListPrice,
                Price = productEntity.Price,
                Price50 = productEntity.Price50,
                Price100 = productEntity.Price100,
                Description = productEntity.Description,
                ISBN = productEntity.ISBN,
                CategoryId = productEntity.CategoryId,
                Category = new BookStore.BLL.DTO.Category
                {
                    Id = productEntity.Category.Id,
                    Name = productEntity.Category.Name,
                    Description = productEntity.Category.Description
                },
                ProductImages = productEntity.ProductImages.Select(pi => new BookStore.BLL.DTO.ProductImage
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageUrl,
                    ProductId = pi.ProductId,
                }).ToList()
            }
        };

        return View(cartItemDTO);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Details(BookStore.BLL.DTO.ShoppingCart shoppingCartDTO)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        await shoppingCartsService.UpdateCartAsync(shoppingCartDTO, userId);

        TempData["success"] = "Cart updated successfully";

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}