using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IProductsService
    {
        Task<IEnumerable<Product>> GetAllProductsWithCategoryAsync();
        Task<ProductViewModel> GetProductForUpsertAsync(int? id);
        Task<bool> UpsertProductAsync(ProductViewModel productVM, List<IFormFile> files);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<BookStore.BLL.DTO.Product>> GetAllCategoryProductImagesAsync(string includeProperties);
        Task<BLL.DTO.Product> GetProductDetailsAsync(int productId, string includeProperties);
        Task<IEnumerable<DTO.Category>> GetAllCategoriesAsync();
    }
}
