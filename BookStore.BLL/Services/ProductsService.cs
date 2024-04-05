using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using BookStore.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class ProductsService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository, 
                                 IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : IProductsService
    {
        public DTO.Product Create(ProductAddEditRequestModel model)
        {
            var product = new DAL.Entities.Product()
            {
                Title = model.Title,
                ISBN = model.ISBN,
                Price = model.Price,
                Price50 = model.Price50,
                ListPrice = model.ListPrice,
                Price100 = model.Price100,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Author = model.Author,
                ProductImages = model.ProductImages?.Select(pi => new DAL.Entities.ProductImage { ImageUrl = pi.ImageUrl }).ToList()
            };

            productsRepository.Add(product);
            unitOfWork.SaveChanges();

            return GetById(product.Id);
        }

        private DTO.Product GetById(int id)
        {
            var dbProduct = productsRepository.Get(p => p.Id == id);

            if (dbProduct == null)
            {
                throw new Exception("Product not found");
            }

            return new DTO.Product
            {
                Id = dbProduct.Id,
                Title = dbProduct.Title,
                ISBN = dbProduct.ISBN,
                Price = dbProduct.Price,
                Price50 = dbProduct.Price50,
                ListPrice = dbProduct.ListPrice,
                Price100 = dbProduct.Price100,
                Description = dbProduct.Description,
                CategoryId = dbProduct.CategoryId,
                Author = dbProduct.Author,
                ProductImages = dbProduct.ProductImages?.Select(pi => new DTO.ProductImage { Id = pi.Id, ImageUrl = pi.ImageUrl }).ToList()
            };
        }

        public void Delete(int id)
        {
            var product = productsRepository.Get(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            productsRepository.Remove(product);
            unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.Product> GetProducts()
        {
            var dbProducts = productsRepository.GetAll();

            var result = new List<DTO.Product>();
            foreach (var product in dbProducts)
            {
                result.Add(new DTO.Product
                {
                    Id = product.Id,
                    Title = product.Title,
                    ISBN = product.ISBN,
                    Price = product.Price,
                    Price50 = product.Price50,
                    ListPrice = product.ListPrice,
                    Price100 = product.Price100,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Author = product.Author,
                    ProductImages = product.ProductImages?.Select(pi => new DTO.ProductImage { Id = pi.Id, ImageUrl = pi.ImageUrl }).ToList()
                });
            }
            return result;
        }

        public void Update(ProductAddEditRequestModel model)
        {
            var productToUpdate = productsRepository.Get(p => p.Id == model.Id);
            if (productToUpdate == null)
            {
                throw new Exception("Product not found");
            }

            productToUpdate.Title = model.Title;
            productToUpdate.ISBN = model.ISBN;
            productToUpdate.Price = model.Price;
            productToUpdate.Price50 = model.Price50;
            productToUpdate.ListPrice = model.ListPrice;
            productToUpdate.Price100 = model.Price100;
            productToUpdate.Description = model.Description;
            productToUpdate.CategoryId = model.CategoryId;
            productToUpdate.Author = model.Author;
            productToUpdate.ProductImages = model.ProductImages?.Select(pi => new DAL.Entities.ProductImage { ImageUrl = pi.ImageUrl }).ToList();

            productsRepository.Update(productToUpdate);
            unitOfWork.SaveChanges();
        }

        public void RemoveRange(IEnumerable<DTO.Product> products)
        {
            var product = productsRepository.GetAll().ToList();

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            productsRepository.RemoveRange(product);
            unitOfWork.SaveChanges();
        }

        public IEnumerable<DTO.Product> GetAllProductsWithCategory()
        {
            var dbProducts = productsRepository.GetAll(includeProperties: "Category");

            foreach (var product in dbProducts)
            {
                var productDTO = new DTO.Product
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Title = product.Title,
                    Author = product.Author,
                    Description = product.Description,
                    ISBN = product.ISBN,
                    ListPrice = product.ListPrice,
                    Price = product.Price,
                    Price50 = product.Price50,
                    Price100 = product.Price100,

                    Category = new DTO.Category
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    }
                };

                yield return productDTO;
            }
        }

        public ProductVM GetProductForUpsert(int? id)
        {
            var productVM = new ProductVM
            {
                CategoryList = categoriesRepository.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = id == null || id == 0
                    ? new DAL.Entities.Product()
                    : unitOfWork.Products.Get(u => u.Id == id, includeProperties: "ProductImages")
            };

            return productVM;
        }

        public bool UpsertProduct(ProductVM productVM, List<IFormFile> files)
        {
            if (productVM == null)
                throw new ArgumentNullException(nameof(productVM));

            if (productVM.Product == null)
                throw new ArgumentException("Product cannot be null.");

            if (string.IsNullOrEmpty(productVM.Product.Title))
                throw new ArgumentException("Product title is required.");

            if (productVM.Product.Price < 0)
                throw new ArgumentException("Product price must be non-negative.");

            if (productVM.Product.CategoryId == null)
                throw new ArgumentException("At least one category must be selected for the product.");

            try
            {
                bool isProductCreated = false;

                if (productVM.Product.Id == 0)
                {
                    productsRepository.Add(productVM.Product);
                    isProductCreated = true;
                }
                else
                {
                    productsRepository.Update(productVM.Product);
                }

                unitOfWork.SaveChanges();

                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var productImage = new DAL.Entities.ProductImage
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<DAL.Entities.ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    productsRepository.Update(productVM.Product);
                    unitOfWork.SaveChanges();
                }

                return isProductCreated;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upsert product.", ex);
            }
        }

        public IEnumerable<DAL.Entities.Product> GetAllProducts(string includeProperties)
        {
            return productsRepository.GetAll(includeProperties: "Category");
        }

        public void DeleteProduct(int id)
        {
            var productToBeDeleted = productsRepository.Get(u => u.Id == id);

            if (productToBeDeleted == null)
            {
                throw new Exception("Product not found");
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            productsRepository.Remove(productToBeDeleted);
            unitOfWork.SaveChanges();
        }

        public IEnumerable<BookStore.DAL.Entities.Product> GetAllCategoryProductImages(string includeProperties)
        {
            return productsRepository.GetAll(includeProperties: "Category,ProductImages");
        }

        public DAL.Entities.Product GetProductDetails(int productId, string includeProperties)
        {
            return productsRepository.Get(u => u.Id == productId, includeProperties);
        }

        public double CalculateTotalPrice(DAL.Entities.ShoppingCart shoppingCart)
        {
            // Fetch the product details
            var product = shoppingCart.Product;

            if (product == null)
            {
                throw new Exception("Product not found in shopping cart");
            }

            // Calculate the total price based on the quantity
            double totalPrice = shoppingCart.Count switch
            {
                var count when count <= 50 => product.Price * count,
                var count when count <= 100 => product.Price50 * count,
                var count when count > 100 => product.Price100 * count
            };

            return totalPrice;
        }

        public DAL.Entities.Product GetProductById(int productId)
        {
            var product = productsRepository.GetProductById(productId);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            return new DAL.Entities.Product
            {
                Id = product.Id,
                Title = product.Title,
                ISBN = product.ISBN,
                Price = product.Price,
                Author = product.Author,
                Category = product.Category,
                CategoryId = product.CategoryId,
                Description = product.Description,
                ListPrice = product.ListPrice,
                Price50 = product.Price50,
                Price100 = product.Price100,
                ProductImages = product.ProductImages,
            };
        }
    }
}