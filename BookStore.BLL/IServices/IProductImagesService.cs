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
        Task<IEnumerable<BLL.DTO.ProductImage>> GetProductImagesAsync();
        Task<int> DeleteImageAsync(int imageId);
    }
}
