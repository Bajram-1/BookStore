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
        public IEnumerable<DTO.Company> GetAllCompanies()
        {
            var dbCompanies = companiesRepository.GetAll();
            var result = new List<DTO.Company>();
            foreach (var x in dbCompanies)
            {
                result.Add(new DTO.Company
                {
                    Id = x.Id,
                    Name = x.Name,
                    StreetAddress = x.StreetAddress,
                    City = x.City,
                    State = x.State,
                    PhoneNumber = x.PhoneNumber,
                    PostalCode = x.PostalCode
                });
            }
            return result;
        }

        public CompanyAddEditRequestModel GetCompanyById(int id)
        {
            var dbCompany = companiesRepository.GetById(id) ?? throw new Exception("Company not found");
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

        public void AddCompany(CompanyAddEditRequestModel companyObj)
        {
            var existByName = GetByName(companyObj.Name);

            if (existByName != null)
            {
                throw new Exception("Another category with this name already exists");
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

            companiesRepository.Add(company);
            unitOfWork.SaveChanges();
        }

        public void UpdateCompany(int id, CompanyAddEditRequestModel model)
        {
            var existsByName = GetByName(model.Name);

            if (existsByName != null && existsByName.Id != id)
            {
                throw new Exception("Another category with this name already exists");
            }

            var company = companiesRepository.GetById(id);

            company.Name = model.Name;
            company.PhoneNumber = model.PhoneNumber;
            company.StreetAddress = model.StreetAddress;
            company.City = model.City;
            company.State = model.State;
            company.PostalCode = model.PostalCode;

            unitOfWork.SaveChanges();
        }

        public void DeleteCompany(int id)
        {
            var companyToBeDeleted = companiesRepository.Get(u => u.Id == id);

            if (companyToBeDeleted != null)
            {
                companiesRepository.Remove(companyToBeDeleted);
                unitOfWork.SaveChanges();
            }
        }

        public void DeleteCompanies()
        {
            var companyToBeDeleted = companiesRepository.GetAll();

            if (companyToBeDeleted != null)
            {
                companiesRepository.RemoveRange(companyToBeDeleted);
                unitOfWork.SaveChanges();
            }
        }

        public DTO.Company GetByName(string name)
        {
            var dbCategory = companiesRepository.GetByName(name);
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