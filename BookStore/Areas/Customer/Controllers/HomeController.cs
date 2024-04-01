using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.Common;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IProductsService productsService, IShoppingCartsService shoppingCartsService) : Controller
{
    public IActionResult Index()
    {
        IEnumerable<BookStore.DAL.Entities.Product> productList = productsService.GetAllCategoryProductImages(includeProperties: "Category,ProductImages");
        return View(productList);
    }

    [HttpGet]
    public IActionResult Details(int productId)
    {
        BookStore.DAL.Entities.ShoppingCart cart = new()
        {
            Product = productsService.GetProductDetails(productId, includeProperties: "Category,ProductImages"),
            Count = 1,
            ProductId = productId
        };
        return View(cart);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(BookStore.DAL.Entities.ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCartsService.UpdateCart(shoppingCart, userId);

        TempData["success"] = "Cart updated successfully";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}