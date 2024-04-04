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
    public class ProductImagesService(IProductImagesRepository productImagesRepository, IUnitOfWork unitOfWork, 
                                      IWebHostEnvironment webHostEnvironment) : IProductImagesService
    {
        public DTO.ProductImage Create(ProductImageAddEditRequestModel model)
        {
            var productImage = new DAL.Entities.ProductImage
            {
                ImageUrl = model.ImageUrl,
                ProductId = model.ProductId,
            };

            productImagesRepository.Add(productImage);
            unitOfWork.SaveChanges();

            return GetById(productImage.Id);
        }

        private DTO.ProductImage GetById(int id)
        {
            var dbProductImage = productImagesRepository.Get(p => p.Id == id);

            if (dbProductImage == null)
            {
                throw new Exception("Product image not found");
            }
            return new DTO.ProductImage
            {
                Id = dbProductImage.Id,
                ImageUrl = dbProductImage.ImageUrl,
                ProductId = dbProductImage.ProductId
            };
        }

        public void Delete(int id)
        {
            var productImage = productImagesRepository.Get(p => p.Id == id);

            if (productImage == null)
            {
                throw new Exception("Product image not found");
            }

            productImagesRepository.Remove(productImage);
            unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.ProductImage> GetProductImages()
        {
            var dbProductImages = productImagesRepository.GetAll();

            var result = new List<DTO.ProductImage>();
            foreach (var productImage in dbProductImages)
            {
                result.Add(new DTO.ProductImage
                {
                    Id = productImage.Id,
                    ImageUrl = productImage.ImageUrl,
                    ProductId = productImage.ProductId
                });
            }
            return result;
        }

        public IEnumerable<DTO.ProductImage> GetProductImagesByProductId(int productId)
        {
            var productImages = productImagesRepository.GetProductImagesByProductId(productId);

            return productImages.Select(image => new DTO.ProductImage
            {
                Id = image.Id,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl
            });
        }

        public void Update(int id, ProductImageAddEditRequestModel model)
        {
            var productImageToUpdate = productImagesRepository.Get(p => p.Id == id);
            if (productImageToUpdate == null)
            {
                throw new Exception("Product image not found");
            }

            productImageToUpdate.ImageUrl = model.ImageUrl;
            productImageToUpdate.ProductId = model.ProductId;

            unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.ProductImage> GetAllProductImages()
        {
            var entities = productImagesRepository.GetAll().ToList();
            var dtos = entities.Select(entity => new DTO.ProductImage
            {
                Id = entity.Id,
                ImageUrl= entity.ImageUrl,
                ProductId = entity.ProductId
            }).ToList();

            return dtos;
        }

        public int DeleteImage(int imageId)
        {
            var imageToBeDeleted = productImagesRepository.Get(u => u.Id == imageId);
            if (imageToBeDeleted == null)
                throw new Exception("Image not found");

            int productId = imageToBeDeleted.ProductId;

            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            productImagesRepository.Remove(imageToBeDeleted);
            unitOfWork.SaveChanges();

            return productId;
        }
    }
}