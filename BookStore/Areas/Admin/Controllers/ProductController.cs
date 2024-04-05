using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.DAL.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController(IUnitOfWork unitOfWork, IProductsService productsService, 
                                   ICategoriesService categoriesService, IProductImagesService productImagesService, 
                                   IWebHostEnvironment webHostEnvironment1) : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var productList = productsService.GetAllProductsWithCategory();
                return View(productList);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            try
            {
                var productVM = productsService.GetProductForUpsert(id);
                return View(productVM);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            try
            {
                bool isProductCreated = productsService.UpsertProduct(productVM, files);

                if (isProductCreated)
                {
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    TempData["success"] = "Product updated successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to upsert product.";
            }

            var categoryList = categoriesService.GetCategories()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            productVM.CategoryList = categoryList;

            return View(productVM);
        }

        public IActionResult DeleteImage(int imageId)
        {
            int productId;
            try
            {
                productId = productImagesService.DeleteImage(imageId);
                TempData["success"] = "Image deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to delete image.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Upsert", new { id = productId });
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<DAL.Entities.Product> objProductList = productsService.GetAllProducts(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                productsService.DeleteProduct(id);
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
        }

        #endregion
    }
}
