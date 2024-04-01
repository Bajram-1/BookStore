using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface IProductImagesRepository
    {
        void Update(ProductImage productImage);
        IEnumerable<ProductImage> GetAll(Expression<Func<ProductImage, bool>>? filter = null, string? includeProperties = null);
        ProductImage Get(Expression<Func<ProductImage, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(ProductImage productImage);
        void Remove(ProductImage productImage);
        void RemoveRange(IEnumerable<ProductImage> productImages);
    }
}