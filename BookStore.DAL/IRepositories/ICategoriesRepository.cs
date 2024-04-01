using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface ICategoriesRepository
    {
        void Create(Category category);
        void Delete(int id);
        Category GetById(int id);
        IEnumerable<Category> GetAll();
    }
}