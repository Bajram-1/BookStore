using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController(ICompaniesService _companyService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var companies = await _companyService.GetAllAsync();
            return View(companies);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new CompanyAddEditRequestModel());
            }
            else
            {
                try
                {
                    var company = await _companyService.GetCompanyByIdAsync(id.Value);
                    return View(company);
                }
                catch
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompanyAddEditRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        await _companyService.AddCompanyAsync(model);
                        TempData["success"] = "Company created successfully.";
                    }
                    else
                    {
                        await _companyService.UpdateCompanyAsync(model.Id, model);
                        TempData["success"] = "Company updated successfully.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                await _companyService.DeleteCompanyAsync(id.Value);
                return Json(new { success = true, message = "Company deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error while deleting company" });
            }
        }
    }
}