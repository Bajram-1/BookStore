using BookStore.BLL.DTO;
using BookStore.BLL.IServices;
using BookStore.Common;
using BookStore.Common.Exceptions;
using BookStore.DAL.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController(IProductsService productsService,
                                   ICategoriesService categoriesService,
                                   IProductImagesService productImagesService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                var productList = await productsService.GetAllProductsWithCategoryAsync();
                return View(productList);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            try
            {
                var productVM = await productsService.GetProductForUpsertAsync(id);
                return View(productVM);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductViewModel productVM, List<IFormFile> files)
        {
            try
            {
                bool isProductCreated = await productsService.UpsertProductAsync(productVM, files);

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
            catch(DuplicateISBNException ex)
            {
                ModelState.AddModelError("Product.ISBN", ex.Message);
            }
            catch(DuplicateTitleAndAuthorException ex)
            {
                ModelState.AddModelError("Product.Title", ex.Message);
                ModelState.AddModelError("Product.Author", ex.Message);
            }

            var categoryList = (await categoriesService.GetCategories())
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            productVM.CategoryList = categoryList;

            return View(productVM);
        }

        public async Task<IActionResult> DeleteImage(int imageId)
        {
            int productId;
            try
            {
                productId = await productImagesService.DeleteImageAsync(imageId);
                TempData["success"] = "Image deleted successfully.";
            }
            catch (Exception)
            {
                TempData["error"] = "Failed to delete image.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Upsert", new { id = productId });
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productDTOList = await productsService.GetAllProductsWithCategoryAsync();
                return Json(new { data = productDTOList });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error while fetching products" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await productsService.DeleteProductAsync(id);
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error while deleting product" });
            }
        }
        #endregion
    }
}