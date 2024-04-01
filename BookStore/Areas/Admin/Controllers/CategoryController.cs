using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController(ICategoriesService categoriesService) : Controller
    {
        public IActionResult Index()
        {
            return View(categoriesService.GetCategories());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryAddEditRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryAddEditRequestModel category)
        {
            if (ModelState.IsValid)
            {
                categoriesService.Create(category);
                TempData["success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = categoriesService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CategoryAddEditRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    categoriesService.Update(id, model);
                    TempData["success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating category: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var category = categoriesService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            categoriesService.Delete(id);
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}