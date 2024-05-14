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
        Task<IEnumerable<DTO.Company>> GetAllAsync();
        Task<CompanyAddEditRequestModel> GetCompanyByIdAsync(int id);
        Task AddCompanyAsync(CompanyAddEditRequestModel companyObj);
        Task UpdateCompanyAsync(int id, CompanyAddEditRequestModel model);
        Task DeleteCompanyAsync(int id);
        Task<DTO.Company> GetByNameAsync(string name);
    }
}
