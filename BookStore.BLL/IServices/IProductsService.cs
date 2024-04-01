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
        Product Create(ProductAddEditRequestModel model);
        void Delete(int id);
        IEnumerable<DTO.Product> GetProducts();
        void Update(ProductAddEditRequestModel model);
        void RemoveRange(IEnumerable<DTO.Product> products);
        ProductVM GetProductForUpsert(int? id);
        bool UpsertProduct(ProductVM productVM, List<IFormFile> files);
        IEnumerable<DAL.Entities.Product> GetAllProducts(string includeProperties);
        void DeleteProduct(int id);
        IEnumerable<DTO.Product> GetAllProductsWithCategory();
        IEnumerable<BookStore.DAL.Entities.Product> GetAllCategoryProductImages(string includeProperties);
        DAL.Entities.Product GetProductDetails(int productId, string includeProperties);
    }
}
