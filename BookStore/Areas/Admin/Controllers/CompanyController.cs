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
        public IActionResult Index()
        {
            var companies = _companyService.GetAllCompanies();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create
                return View(new CompanyAddEditRequestModel());
            }
            else
            {
                // Update
                try
                {
                    var company = _companyService.GetCompanyById(id.Value);
                    return View(company);
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompanyAddEditRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        _companyService.AddCompany(model);
                        TempData["success"] = "Company created successfully.";
                    }
                    else
                    {
                        _companyService.UpdateCompany(model.Id, model);
                        TempData["success"] = "Company updated successfully.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _companyService.GetAllCompanies();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                _companyService.DeleteCompany(id.Value);
                return Json(new { success = true, message = "Company deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error while deleting company" });
            }
        }
    }
}