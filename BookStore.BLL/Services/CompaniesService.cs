using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class CompaniesService(ICompaniesRepository companiesRepository, IUnitOfWork unitOfWork) : ICompaniesService
    {
        public async Task<IEnumerable<DTO.Company>> GetAllAsync()
        {
            var dbCompanies = await companiesRepository.GetAllAsync();
            return dbCompanies.Select(x => new DTO.Company
            {
                Id = x.Id,
                Name = x.Name,
                StreetAddress = x.StreetAddress,
                City = x.City,
                State = x.State,
                PhoneNumber = x.PhoneNumber,
                PostalCode = x.PostalCode
            }).ToList();
        }

        public async Task<CompanyAddEditRequestModel> GetCompanyByIdAsync(int id)
        {
            var dbCompany = await companiesRepository.GetByIdAsync(id) ?? throw new Exception("Company not found");
            return new CompanyAddEditRequestModel
            {
                Id = dbCompany.Id,
                Name = dbCompany.Name,
                PhoneNumber = dbCompany.PhoneNumber,
                StreetAddress = dbCompany.StreetAddress,
                City = dbCompany.City,
                State = dbCompany.State,
                PostalCode = dbCompany.PostalCode,
            };
        }

        public async Task AddCompanyAsync(CompanyAddEditRequestModel companyObj)
        {
            var existByName = await GetByNameAsync(companyObj.Name);

            if (existByName != null)
            {
                throw new Exception("Another company with this name already exists");
            }

            var company = new DAL.Entities.Company
            {
                Name = companyObj.Name,
                PhoneNumber = companyObj.PhoneNumber,
                StreetAddress = companyObj.StreetAddress,
                City = companyObj.City,
                State = companyObj.State,
                PostalCode = companyObj.PostalCode
            };

            await companiesRepository.CreateAsync(company);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(int id, CompanyAddEditRequestModel model)
        {
            var existsByName = await GetByNameAsync(model.Name);

            if (existsByName != null && existsByName.Id != id)
            {
                throw new Exception("Another company with this name already exists");
            }

            var company = await companiesRepository.GetByIdAsync(id) ?? throw new Exception("Company not found");

            company.Name = model.Name;
            company.PhoneNumber = model.PhoneNumber;
            company.StreetAddress = model.StreetAddress;
            company.City = model.City;
            company.State = model.State;
            company.PostalCode = model.PostalCode;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var companyToBeDeleted = await companiesRepository.GetByIdAsync(id);

            if (companyToBeDeleted != null)
            {
                await companiesRepository.DeleteAsync(companyToBeDeleted);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<DTO.Company> GetByNameAsync(string name)
        {
            var dbCategory = await companiesRepository.GetByNameAsync(name);
            if (dbCategory == null)
            {
                return null;
            }
            return new DTO.Company
            {
                Id = dbCategory.Id,
                Name = dbCategory.Name,
                City = dbCategory.City,
                PhoneNumber = dbCategory.PhoneNumber,
                PostalCode = dbCategory.PostalCode,
                State = dbCategory.State,
                StreetAddress = dbCategory.StreetAddress
            };
        }
    }
}