using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface ICategoriesRepository : IBaseRepository<Category, int>
    {
        Task DeleteAsync(int id);
        Task<Category> GetByIdAsync(int id);
        Task<Category> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<int> GetActualDisplayOrderAsync();
    }
}