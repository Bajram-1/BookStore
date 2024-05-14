using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface ICompaniesRepository : IBaseRepository<Company, int>
    {
        Task<Company> GetByNameAsync(string name);
        Task<Company> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync(Expression<Func<Company, bool>> filter = null, string includeProperties = null);
    }
}