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
        public async Task<IActionResult> Index()
        {
            var categories = await categoriesService.GetCategories();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryAddEditRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAddEditRequestModel category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdCategory = await categoriesService.Create(category);
                    TempData["success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("Name", "This category already exists.");
                }
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await categoriesService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryAddEditRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await categoriesService.Update(id, model);
                    TempData["success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await categoriesService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await categoriesService.Delete(id);
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}