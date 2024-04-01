using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface ICompaniesService
    {
        IEnumerable<DTO.Company> GetAllCompanies();
        CompanyAddEditRequestModel GetCompanyById(int id);
        DTO.Company GetByName(string name);
        void AddCompany(CompanyAddEditRequestModel companyObj);
        void UpdateCompany(int id, CompanyAddEditRequestModel model);
        void DeleteCompany(int id);
        void DeleteCompanies();
    }
}
