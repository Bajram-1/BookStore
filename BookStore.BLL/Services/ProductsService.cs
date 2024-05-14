using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.BLL.IServices;
using BookStore.BLL.Services.Singletons;
using BookStore.Common.Exceptions;
using BookStore.DAL.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.BLL.Services
{
    public delegate void ObjectAddedEvent(int id);

    public class ProductsService(IProductsRepository productsRepository, 
                                 ICategoriesRepository categoriesRepository, 
                                 IUnitOfWork unitOfWork, 
                                 IWebHostEnvironment webHostEnvironment,
                                 ICacheService cacheService) : IProductsService
    {
        public static event ObjectAddedEvent OnProductAdded;

        public static event OnEntityAdded OnEntityAdded;
        public static event OnEntityDeleted OnEntityDeleted;
        public static event OnEntityUpdated OnEntityUpdated;

        public async Task<IEnumerable<DTO.Product>> GetAllProductsWithCategoryAsync()
        {
            return await cacheService.GetOrAddAsync("allProductsWithCategory", async () =>
            {
                var dbProducts = await productsRepository.GetAllAsync(includeProperties: "Category");

                return dbProducts.Select(product => new DTO.Product
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
                });
            });
        }

        public async Task<ProductViewModel> GetProductForUpsertAsync(int? id)
        {
            var productVM = new ProductViewModel
            {
                CategoryList = (await categoriesRepository.GetAllAsync())
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }).ToList()
            };

            if (id == null || id == 0)
            {
                productVM.Product = new BLL.DTO.Product();
            }
            else
            {
                var productDAL = await unitOfWork.ProductsRepository.GetAsync(u => u.Id == id, includeProperties: "ProductImages");
                if (productDAL != null)
                {
                    productVM.Product = new BLL.DTO.Product
                    {
                        Id = productDAL.Id,
                        Title = productDAL.Title,
                        Description = productDAL.Description,
                        Price = productDAL.Price,
                        ListPrice = productDAL.ListPrice,
                        Price50 = productDAL.Price50,
                        Price100 = productDAL.Price100,
                        Author = productDAL.Author,
                        ISBN = productDAL.ISBN,
                        CategoryId = productDAL.CategoryId,
                        ProductImages = productDAL.ProductImages.Select(img => new BLL.DTO.ProductImage
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl,
                            ProductId = img.ProductId
                        }).ToList()
                    };
                }
            }

            return productVM;
        }

        public async Task<bool> UpsertProductAsync(ProductViewModel productVM, List<IFormFile> files)
        {
            bool isProductCreated = false;

            try
            {
                var existingProduct = await productsRepository.GetByISBNAsync(productVM.Product.ISBN);
                if (existingProduct != null && existingProduct.Id != productVM.Product.Id)
                {
                    throw new DuplicateISBNException();
                }

                var existingProductWithTitleAndAuthor = await productsRepository.GetByTitleAndAuthorAsync(productVM.Product.Title, productVM.Product.Author);
                if (existingProductWithTitleAndAuthor != null && existingProductWithTitleAndAuthor.Id != productVM.Product.Id)
                {
                    throw new DuplicateTitleAndAuthorException();
                }

                var productDAL = new DAL.Entities.Product
                {
                    Id = productVM.Product.Id,
                    Title = productVM.Product.Title,
                    Description = productVM.Product.Description,
                    Price = productVM.Product.Price,
                    ListPrice = productVM.Product.ListPrice,
                    Price50 = productVM.Product.Price50,
                    Price100 = productVM.Product.Price100,
                    Author = productVM.Product.Author,
                    ISBN = productVM.Product.ISBN,
                    CategoryId = productVM.Product.CategoryId,
                    ProductImages = productVM.Product.ProductImages?.Select(img => new DAL.Entities.ProductImage
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        ProductId = img.ProductId
                    }).ToList() ?? new List<DAL.Entities.ProductImage>()
                };

                if (productVM.Product.Id == 0)
                {
                    await productsRepository.CreateAsync(productDAL);
                    isProductCreated = true;
                }
                else
                {
                    await productsRepository.UpdateAsync(productDAL);
                }

                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync("allProductsWithCategory");

                if (isProductCreated)
                {
                    OnProductAdded?.Invoke(productDAL.Id);
                    OnEntityAdded?.Invoke(new DAL.Entities.AuditLog
                    {
                        EntityId = productDAL.Id.ToString(),
                        EntityName = "Product",
                        LogType = Common.Enums.AuditLogType.Create,
                        Details = Newtonsoft.Json.JsonConvert.SerializeObject(productDAL)
                    });
                }
                else
                {
                    OnEntityUpdated?.Invoke(new DAL.Entities.AuditLog
                    {
                        EntityId = productDAL.Id.ToString(),
                        EntityName = "Product",
                        LogType = Common.Enums.AuditLogType.Update,
                        Details = Newtonsoft.Json.JsonConvert.SerializeObject(productDAL)
                    });
                }

                if (files != null && files.Any())
                {
                    string wwwRootPath = webHostEnvironment.WebRootPath;
                    string productPath = Path.Combine(wwwRootPath, "images", "products", "product-" + productDAL.Id);
                    Directory.CreateDirectory(productPath);

                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(productPath, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var productImage = new DAL.Entities.ProductImage
                        {
                            ImageUrl = @"\images\products\product-" + productDAL.Id + @"\" + fileName,
                            ProductId = productDAL.Id,
                        };

                        productDAL.ProductImages.Add(productImage);
                    }

                    await productsRepository.UpdateAsync(productDAL);
                    await unitOfWork.SaveChangesAsync();
                }

                return isProductCreated;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during product upsert: " + ex.Message);
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var productToBeDeleted = await productsRepository.GetAsync(u => u.Id == id);

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

            await productsRepository.DeleteAsync(productToBeDeleted);
            await unitOfWork.SaveChangesAsync();
            OnEntityDeleted?.Invoke(new DAL.Entities.AuditLog
            {
                EntityId = id.ToString(),
                EntityName = "Product",
                Details = "Product was deleted",
                LogType = Common.Enums.AuditLogType.Delete
            });
            await cacheService.RemoveAsync("allProductsWithCategory");
        }

        public async Task<IEnumerable<BookStore.BLL.DTO.Product>> GetAllCategoryProductImagesAsync(string includeProperties)
        {
            var dbProducts = await productsRepository.GetAllAsync(includeProperties: "Category,ProductImages");

            return dbProducts.Select(product => new BookStore.BLL.DTO.Product
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

                Category = new BookStore.BLL.DTO.Category
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                ProductImages = product.ProductImages.Select(image => new BookStore.BLL.DTO.ProductImage
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl,
                    ProductId = image.ProductId
                }).ToList()
            }).ToList();
        }

        public async Task<BLL.DTO.Product> GetProductDetailsAsync(int productId, string includeProperties)
        {
            var dbProduct = await productsRepository.GetAsync(u => u.Id == productId, includeProperties);

            return new BLL.DTO.Product
            {
                Id = dbProduct.Id,
                CategoryId = dbProduct.CategoryId,
                Title = dbProduct.Title,
                Author = dbProduct.Author,
                Description = dbProduct.Description,
                ISBN = dbProduct.ISBN,
                ListPrice = dbProduct.ListPrice,
                Price = dbProduct.Price,
                Price50 = dbProduct.Price50,
                Price100 = dbProduct.Price100,

                Category = new BLL.DTO.Category
                {
                    Id = dbProduct.Category.Id,
                    Name = dbProduct.Category.Name
                },
                ProductImages = dbProduct.ProductImages.Select(image => new BLL.DTO.ProductImage
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl,
                    ProductId = image.ProductId
                }).ToList()
            };
        }

        public async Task<IEnumerable<DTO.Category>> GetAllCategoriesAsync()
        {
            var categories = await categoriesRepository.GetAllAsync();
            return categories.Select(c => new DTO.Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DisplayOrder = c.DisplayOrder
            });
        }
    }
}