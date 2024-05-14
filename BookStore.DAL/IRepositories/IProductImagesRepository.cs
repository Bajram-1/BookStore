using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface IProductImagesRepository : IBaseRepository<ProductImage, int>
    {
        Task<IEnumerable<ProductImage>> GetAllAsync();
    }
}