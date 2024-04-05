using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IProductImagesService
    {
        DTO.ProductImage Create(ProductImageAddEditRequestModel model);
        void Delete(int id);
        IEnumerable<DTO.ProductImage> GetProductImages();
        void Update(int id, ProductImageAddEditRequestModel model);
        IEnumerable<DTO.ProductImage> GetAllProductImages();
        IEnumerable<DTO.ProductImage> GetProductImagesByProductId(int productId);
        int DeleteImage(int imageId);
    }
}
