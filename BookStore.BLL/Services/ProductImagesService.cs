using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class ProductImagesService(IProductImagesRepository productImagesRepository, 
                                      IUnitOfWork unitOfWork,
                                      IWebHostEnvironment webHostEnvironment) : IProductImagesService
    {
        public async Task<IEnumerable<BLL.DTO.ProductImage>> GetProductImagesAsync()
        {
            var productImages = await productImagesRepository.GetAllAsync();

            var productImageDTOs = productImages.Select(pi => new BLL.DTO.ProductImage
            {
                Id = pi.Id,
                ImageUrl = pi.ImageUrl,
                ProductId = pi.ProductId,
            });

            return productImageDTOs;
        }

        public async Task<int> DeleteImageAsync(int imageId)
        {
            var imageToBeDeleted = await productImagesRepository.GetAsync(u => u.Id == imageId);
            if (imageToBeDeleted == null)
                throw new Exception("Image not found");

            int productId = imageToBeDeleted.ProductId;

            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            await productImagesRepository.DeleteAsync(imageToBeDeleted);
            await unitOfWork.SaveChangesAsync();

            return productId;
        }
    }
}