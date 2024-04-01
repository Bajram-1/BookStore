using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface ICompaniesRepository
    {
        void Update(Company company);
        IEnumerable<Company> GetAll(Expression<Func<Company, bool>>? filter = null, string? includeProperties = null);
        Company Get(Expression<Func<Company, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(Company company);
        void Remove(Company company);
        void RemoveRange(IEnumerable<Company> companies);
        Company? GetByName(string name);
        Company GetById(int id);
    }
}