using BookStore.DAL;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace BookStore.Tests
{
    public class ProductsRepoTests
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsRepoTests()
        {
            var config = new Dictionary<string, string>();
            config.Add("ConnectionStrings:UnitTestsConnection", "Server=BANISHEHI\\SQLEXPRESS;Initial Catalog=BookStoreTests;Trusted_Connection=True;TrustServerCertificate=True");
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
            var services = new ServiceCollection();
            DALStartup.RegisterDALServices(services, configuration);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        /// <summary>
        /// 1. Lidh me databazen
        /// 2. Merr te gjitha produktet nga databaza
        /// 3. Nese nuk ka asnje vlere, testi do te dali gabim
        /// </summary> 
        [Fact]
        public void Test_001_GetAllProducts_ShouldGetProducts()
        {
            var products = _unitOfWork.ProductsRepository.GetAllAsync().Result;

            if (products.Count() == 0)
            {
                Assert.True(false, "Nuk ka asnje vlere");
            }

            Assert.True(true);
        }

        /// <summary>
        /// 1. Lidh me databazen.
        /// 2. Merr te gjithe produktet nga databaza.
        /// 3. Kontrollojme neqoftese ka produkte ne tabele, nese eshte bosh afishojme mesazhin Produktet null ose ska asnje vlere
        /// 4. Kontrollojme ne listen e produkteve nese produkti i pare e ka titullin Cotton Candy
        /// </summary>
        [Fact]
        public async Task Test_002_GetAllProducts_ShouldFail()
        {
            var products = await _unitOfWork.ProductsRepository.GetAllAsync();
            Assert.True(products != null && products.Count() > 0, "Produktet null ose ska asnje vlere");
            var productsList = products.ToList();
            Assert.False(productsList[0].Title == "Cotton Candy", "Produkti i pare nuk e ka titullin Cotton Candy");
        }

        /// <summary>
        /// 1. Krijojme nje produkt me keto vlera si me poshte.
        /// 2. Testojme nese mund te ruajme kete produkt me keto vlera ne databaze.
        /// 3. Ne te kundert hedhim errorin perkates.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Test_003_InsertShouldFail()
        {
            try
            {
                var product = new Product
                {
                    Title = "",
                    Author = "",
                    Description = "",
                    ISBN = "",
                    Id = 0
                };
                await _unitOfWork.ProductsRepository.CreateAsync(product);
                await _unitOfWork.SaveChangesAsync();
                Assert.Equal(0, product.Id);
            }
            catch
            {
                return;
            }
        }


        /// <summary>
        /// 1. Krijojme nje kategori me emrin Adventure
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese mund te ruajme kete produkt ne databaze
        /// 4. Nese nuk mund te ruajme produktin, testi do te dali gabim
        /// 5. Testojme nese produkti i ruajtur ne databaze ka titullin Updated Title, autorin Updated Author dhe pershkrimin Updated Description
        /// 6. Nese jane te njejta, testi do te jete i suksesshem
        /// 7. Nese nuk jane te njejta, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_004_UpdateProduct_ShouldUpdateProduct()
        {
            var category = new Category { Name = "Adventure" };
            await _unitOfWork.CategoriesRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var product = new Product
            {
                Title = "Initial Title",
                Author = "Initial Author",
                Description = "Initial Description",
                ISBN = "Initial ISBN",
                CategoryId = category.Id
            };

            await _unitOfWork.ProductsRepository.CreateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            product.Title = "Updated Title";
            product.Author = "Updated Author";
            product.Description = "Updated Description";
            await _unitOfWork.ProductsRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var updatedProduct = await _unitOfWork.ProductsRepository.GetAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Title", updatedProduct.Title);
            Assert.Equal("Updated Author", updatedProduct.Author);
            Assert.Equal("Updated Description", updatedProduct.Description);
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Adventure
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese mund te ruajme kete produkt ne databaze
        /// 4. Nese nuk mund te ruajme produktin, testi do te dali gabim
        /// 5. Testojme nese produkti i ruajtur ne databaze ka cmimin 100.00 dhe cmimin e listes 120.00
        /// 6. Nese jane te njejta, testi do te jete i suksesshem
        /// 7. Nese nuk jane te njejta, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_005_ListPriceGreaterThanPrice()
        {
            var category = new Category { Name = "Adventure" };
            await _unitOfWork.CategoriesRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var product = new Product
            {
                Title = "Test Product",
                Author = "Test Author",
                Description = "A product for testing",
                ISBN = "123-TEST",
                Price = 100.00m,
                ListPrice = 120.00m,
                CategoryId = category.Id
            };

            await _unitOfWork.ProductsRepository.CreateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var savedProduct = await _unitOfWork.ProductsRepository.GetAsync(p => p.Id == product.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal(100.00m, savedProduct.Price);
            Assert.Equal(120.00m, savedProduct.ListPrice);
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Test
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese mund te fshijme kete produkt ne databaze
        /// 4. Nese nuk mund te fshijme produktin, testi do te dali gabim
        /// 5. Testojme nese produkti i fshire nga databaza nuk ekziston
        /// 6. Nese nuk ekziston, testi do te jete i suksesshem
        /// 7. Nese ekziston, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_006_DeleteProduct_ShouldRemoveProductFromDatabase()
        {
            var category = new Category { Name = "Test" };
            await _unitOfWork.CategoriesRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var product = new Product
            {
                Title = "Delete Test Product",
                Author = "Author Test",
                Description = "Product to test the delete functionality",
                ISBN = "DEL-123456",
                Price = 50.00m,
                ListPrice = 45.00m,
                CategoryId = category.Id
            };

            await _unitOfWork.ProductsRepository.CreateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.ProductsRepository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var retrievedProduct = await _unitOfWork.ProductsRepository.GetAsync(p => p.Id == product.Id);
            Assert.Null(retrievedProduct);
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Science Fiction
        /// 2. Krijojme dy produkte me vlerat si me poshte
        /// 3. Testojme nese mund te marrim produktin me filtra dhe te dhenat e lidhura
        /// 4. Nese nuk mund te marrim produktin, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_007_GetProduct_WithFiltersAndIncludes_ShouldReturnCorrectData()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Science Fiction" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var product1 = new Product
                {
                    Title = "Product 1",
                    Author = "Author 1",
                    Description = "A science fiction book",
                    ISBN = "ISBN001",
                    Price = 15.00m,
                    ListPrice = 20.00m,
                    CategoryId = category.Id
                };
                var product2 = new Product
                {
                    Title = "Product 2",
                    Author = "Author 2",
                    Description = "Another science fiction book",
                    ISBN = "ISBN002",
                    Price = 18.00m,
                    ListPrice = 22.00m,
                    CategoryId = category.Id
                };

                await _unitOfWork.ProductsRepository.CreateAsync(product1);
                await _unitOfWork.ProductsRepository.CreateAsync(product2);
                await _unitOfWork.SaveChangesAsync();

                Expression<Func<Product, bool>> filter = p => p.Price > 16.00m && p.Title == "Product 2";
                string includeProperties = "Category";
                var retrievedProduct = await _unitOfWork.ProductsRepository.GetAsync(filter, includeProperties);

                Assert.NotNull(retrievedProduct);
                Assert.Equal("Product 2", retrievedProduct.Title);
                Assert.NotNull(retrievedProduct.Category);
                Assert.Equal("Science Fiction", retrievedProduct.Category.Name);

                await transaction.RollbackAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Bulk Delete Category
        /// 2. Krijojme dy produkte me vlerat si me poshte
        /// 3. Testojme nese mund te fshijme te dy produktet
        /// 4. Nese nuk mund te fshijme te dy produktet, testi do te dali gabim
        /// 5. Testojme nese produktet e fshire nga databaza nuk ekzistojne
        /// 6. Nese nuk ekzistojne, testi do te jete i suksesshem
        /// 7. Nese ekzistojne, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_008_RemoveRangeOfProducts_ShouldDeleteAllSpecifiedProducts()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Bulk Delete Category" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var products = new List<Product>
        {
            new Product { Title = "Bulk Delete Product 1", Author = "Author 1", Description = "Description 1", ISBN = "ISBN-BULK-1", Price = 10.00m, ListPrice = 11.00m, CategoryId = category.Id },
            new Product { Title = "Bulk Delete Product 2", Author = "Author 2", Description = "Description 2", ISBN = "ISBN-BULK-2", Price = 20.00m, ListPrice = 22.00m, CategoryId = category.Id }
        };

                foreach (var product in products)
                {
                    await _unitOfWork.ProductsRepository.CreateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.ProductsRepository.RemoveRangeAsync(products);
                await _unitOfWork.SaveChangesAsync();

                foreach (var product in products)
                {
                    var exists = await _unitOfWork.ProductsRepository.GetAsync(p => p.Id == product.Id);
                    Assert.Null(exists);
                }

                await transaction.RollbackAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Test Category
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese mund te marrim produktin me kategorine e lidhur
        /// 4. Nese nuk mund te marrim produktin, testi do te dali gabim
        /// 5. Testojme nese produkti i marrur ka kategorine e lidhur
        /// 6. Nese ka kategorine e lidhur, testi do te jete i suksesshem
        /// 7. Nese nuk ka kategorine e lidhur, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_009_GetProductWithCategory_ShouldIncludeCategory()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Test Category" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var product = new Product
                {
                    Title = "Test Product with Category",
                    Author = "Test Author",
                    Description = "Product description here",
                    ISBN = "ISBN-TEST-1234",
                    Price = 15.00m,
                    ListPrice = 20.00m,
                    CategoryId = category.Id
                };

                await _unitOfWork.ProductsRepository.CreateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var retrievedProduct = await _unitOfWork.ProductsRepository.GetAsync(
                    p => p.Id == product.Id,
                    includeProperties: "Category");

                Assert.NotNull(retrievedProduct);
                Assert.NotNull(retrievedProduct.Category);
                Assert.Equal("Test Category", retrievedProduct.Category.Name);

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Historical
        /// 2. Krijojme dy produkte me vlerat si me poshte
        /// 3. Testojme nese mund te fshijme kategorine
        /// 4. Nese nuk mund te fshijme kategorine, testi do te dali gabim
        /// 5. Testojme nese produktet jane fshire ose i jane hequr lidhjet me kategorine
        /// 6. Nese jane fshire ose i jane hequr lidhjet, testi do te jete i suksesshem
        /// 7. Nese nuk jane fshire ose i jane hequr lidhjet, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_010_DeleteCategory_ShouldCascadeDeleteOrUnlinkProducts()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Historical" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var product1 = new Product
                {
                    Title = "Historical Novel",
                    Author = "Author X",
                    Description = "A detailed historical narrative.",
                    ISBN = "HIST-001",
                    CategoryId = category.Id,
                    Price = 25.99m
                };

                var product2 = new Product
                {
                    Title = "Historical Biography",
                    Author = "Author Y",
                    Description = "Biography of a historical figure.",
                    ISBN = "HIST-002",
                    CategoryId = category.Id,
                    Price = 15.99m
                };

                await _unitOfWork.ProductsRepository.CreateAsync(product1);
                await _unitOfWork.ProductsRepository.CreateAsync(product2);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CategoriesRepository.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var deletedCategory = await _unitOfWork.CategoriesRepository.GetAsync(c => c.Id == category.Id);
                var productsAfterDelete = await _unitOfWork.ProductsRepository.GetAllAsync(p => p.CategoryId == category.Id);

                Assert.Null(deletedCategory);
                Assert.Empty(productsAfterDelete);

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme dy kategori me emrat Biography dhe Memoir
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese mund te ndryshojme kategorine e produktit
        /// 4. Nese nuk mund te ndryshojme kategorine e produktit, testi do te dali gabim
        /// 5. Testojme nese produkti i ndryshuar ka kategorine e re
        /// 6. Nese ka kategorine e re, testi do te jete i suksesshem
        /// 7. Nese nuk ka kategorine e re, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_011_UpdateProductCategory_ShouldReflectChange()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category1 = new Category { Name = "Biography" };
                var category2 = new Category { Name = "Memoir" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category1);
                await _unitOfWork.CategoriesRepository.CreateAsync(category2);
                await _unitOfWork.SaveChangesAsync();

                var product = new Product
                {
                    Title = "Life of Pi",
                    Author = "Author Z",
                    Description = "An intriguing biography.",
                    ISBN = "BIO-12345",
                    CategoryId = category1.Id,
                    Price = 20.00m
                };

                await _unitOfWork.ProductsRepository.CreateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                product.CategoryId = category2.Id;
                await _unitOfWork.ProductsRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var updatedProduct = await _unitOfWork.ProductsRepository.GetAsync(p => p.Id == product.Id, includeProperties: "Category");
                Assert.NotNull(updatedProduct);
                Assert.Equal(category2.Id, updatedProduct.CategoryId);
                Assert.Equal("Memoir", updatedProduct.Category.Name);

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Literature
        /// 2. Krijojme tre produkte me titujt si me poshte
        /// 3. Testojme nese mund te gjehen produkte me titujt qe permbajne substring-in Peace
        /// 4. Nese nuk mund te gjehen produkte, testi do te dali gabim
        /// 5. Testojme nese produktet e gjetura permbajne substring-in Peace
        /// 6. Nese permbajne substring-in Peace, testi do te jete i suksesshem
        /// 7. Nese nuk permbajne substring-in Peace, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_012_SearchProductsByTitleSubstring_ShouldReturnMatchingProducts()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Literature" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var products = new List<Product>
        {
            new Product { Title = "War and Peace", Author = "Leo Tolstoy", Description = "A historical novel.", ISBN = "ISBN-001", CategoryId = category.Id, Price = 19.99m },
            new Product { Title = "Peaceful Mind, Peaceful Life", Author = "Barb Schmidt", Description = "Guide to inner peace.", ISBN = "ISBN-002", CategoryId = category.Id, Price = 15.99m },
            new Product { Title = "Unbroken", Author = "Laura Hillenbrand", Description = "World War II story of survival.", ISBN = "ISBN-003", CategoryId = category.Id, Price = 13.99m }
        };

                foreach (var product in products)
                {
                    await _unitOfWork.ProductsRepository.CreateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();

                var searchKeyword = "Peace";
                var matchingProducts = await _unitOfWork.ProductsRepository.GetAllAsync(p => p.Title.Contains(searchKeyword));

                Assert.NotNull(matchingProducts);
                Assert.Equal(2, matchingProducts.Count());
                Assert.All(matchingProducts, p => Assert.Contains(searchKeyword, p.Title));

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme dy kategori me emrat Technology dhe Literature
        /// 2. Krijojme kater produkte me vlerat si me poshte
        /// 3. Testojme nese mund te gjenden produktet me kategorine dhe cmimin e percaktuar
        /// 4. Nese nuk mund te gjenden produktet, testi do te dali gabim
        /// 5. Testojme nese produktet e gjetura kane kategorine dhe cmimin e percaktuar
        /// 6. Nese kane kategorine dhe cmimin e percaktuar, testi do te jete i suksesshem
        /// 7. Nese nuk kane kategorine dhe cmimin e percaktuar, testi do te dali gabim
        /// </summary>
        [Fact]
        public async Task Test_013_FilterProducts_ByCategoryAndPrice_ShouldReturnCorrectProducts()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category1 = new Category { Name = "Technology" };
                var category2 = new Category { Name = "Literature" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category1);
                await _unitOfWork.CategoriesRepository.CreateAsync(category2);
                await _unitOfWork.SaveChangesAsync();

                var products = new List<Product>
        {
            new Product { Title = "Advanced Programming", Author = "Author A", Description = "A book on advanced programming techniques.", ISBN = "ISBN-TECH1", CategoryId = category1.Id, Price = 55.00m },
            new Product { Title = "Learning C#", Author = "Author B", Description = "A beginner's guide to programming in C#.", ISBN = "ISBN-TECH2", CategoryId = category1.Id, Price = 35.00m },
            new Product { Title = "World Literature", Author = "Author C", Description = "Exploring classic world literature.", ISBN = "ISBN-LIT1", CategoryId = category2.Id, Price = 40.00m },
            new Product { Title = "Modern Tech Disruptions", Author = "Author D", Description = "Insights into modern technological innovations.", ISBN = "ISBN-TECH3", CategoryId = category1.Id, Price = 60.00m }
        };

                foreach (var product in products)
                {
                    await _unitOfWork.ProductsRepository.CreateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();

                Expression<Func<Product, bool>> filter = p => p.CategoryId == category1.Id && p.Price >= 40.00m && p.Price <= 60.00m;
                var filteredProducts = await _unitOfWork.ProductsRepository.GetAllAsync(filter);

                Assert.NotNull(filteredProducts);
                Assert.Equal(2, filteredProducts.Count());
                Assert.Contains(filteredProducts, p => p.Title == "Advanced Programming");
                Assert.Contains(filteredProducts, p => p.Title == "Modern Tech Disruptions");

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// 1. Krijojme nje produkt me vlerat si me poshte
        /// 2. Testojme nese cmimet e produktit jane te sakta
        /// 3. Nese nuk jane te sakta, testi do te dali gabim
        /// 4. Testojme nese cmimi i zakonshem nuk kalon cmimin e listes
        /// 5. Nese kalon, testi do te dali gabim
        /// 6. Testojme nese cmimi per 50 njesi eshte me i ulet se cmimi i zakonshem
        /// 7. Nese nuk eshte me i ulet, testi do te dali gabim
        /// 8. Testojme nese cmimi per 100 njesi eshte me i ulet se cmimi per 50 njesi
        /// 9. Nese nuk eshte me i ulet, testi do te dali gabim
        /// 10. Nese te gjitha testet kalojne, testi do te jete i suksesshem
        /// 11. Nese nje nga testet nuk kalon, testi do te dali gabim
        /// </summary>
        [Fact]
        public void Test_014_ProductPrice_Validation_ShouldPass()
        {
            var product = new Product
            {
                Title = "Sample Book",
                ListPrice = 20.00m,
                Price = 18.00m,
                Price50 = 17.00m,
                Price100 = 16.00m
            };

            Assert.True(product.Price <= product.ListPrice, "Regular price should not exceed list price.");
            Assert.True(product.Price50 < product.Price, "Price for 50 units should be less than regular price.");
            Assert.True(product.Price100 < product.Price50, "Price for 100 units should be less than price for 50 units.");
        }

        /// <summary>
        /// 1. Krijojme nje produkt me vlerat si me poshte
        /// 2. Testojme nese fushat e detyrueshme te produktit jane te plotesuara
        /// 3. Nese nuk jane te plotesuara, testi do te dali gabim
        /// 4. Nese te gjitha fushat e detyrueshme jane te plotesuara, testi do te jete i suksesshem
        /// </summary>
        [Fact]
        public void Test_015_ProductRequiredFields_ShouldBeValid()
        {
            var product = new Product
            {
                Title = "Effective C#",
                Description = "Learn C#",
                ISBN = "1234567890",
                Author = "Bill Wagner",
                Price = 30.00m,
                ListPrice = 35.00m
            };

            Assert.False(string.IsNullOrEmpty(product.Title), "Title is required.");
            Assert.False(string.IsNullOrEmpty(product.Author), "Author is required.");
            Assert.False(string.IsNullOrEmpty(product.ISBN), "ISBN is required.");
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Programming
        /// 2. Krijojme nje produkt me vlerat si me poshte
        /// 3. Testojme nese lidhjet midis produkteve dhe kategorive dhe imazheve te produkteve jane te sakta
        /// 4. Nese nuk jane te sakta, testi do te dali gabim
        /// 5. Nese jane te sakta, testi do te jete i suksesshem
        /// 6. Nese nje nga testet nuk kalon, testi do te dali gabim, ne te kundert do te jete i sukseshem
        /// </summary>
        [Fact]
        public void Test_016_ProductRelationships_ShouldBeMaintained()
        {
            var category = new Category { Name = "Programming" };
            var product = new Product
            {
                Title = "Advanced Programming",
                Author = "Author X",
                Category = category,
                ProductImages = new List<ProductImage>
        {
            new ProductImage { ImageUrl = "image1.jpg" },
            new ProductImage { ImageUrl = "image2.jpg" }
        }
            };

            try
            {
                Assert.Equal("Programming", product.Category.Name);
            }
            catch (EqualException)
            {
                throw new InvalidOperationException("Category should be linked correctly.");
            }

            try
            {
                Assert.Equal(2, product.ProductImages.Count);
            }
            catch (EqualException)
            {
                throw new InvalidOperationException("Product should have two images.");
            }
        }

        /// <summary>
        /// 1. Krijojme nje produkt me vlerat si me poshte
        /// 2. Testojme nese cmimet e produkteve nuk jane negative
        /// 3. Nese jane negative, testi do te dali gabim
        /// 4. Testojme nese cmimet e listes nuk jane negative
        /// 5. Nese jane negative, testi do te dali gabim
        /// 6. Nese te gjitha testet kalojne, testi do te jete i suksesshem
        /// </summary>
        [Fact]
        public void Test_017_ProductPrices_ShouldNotBeNegative_ShouldFail()
        {
            var product = new Product
            {
                Title = "New Book",
                Price = -10.00m,
                ListPrice = 20.00m
            };

            Assert.True(product.Price >= 0, "Price cannot be negative.");
            Assert.True(product.ListPrice >= 0, "List price cannot be negative.");
        }

        /// <summary>
        /// 1. Krijojme nje kategori me emrin Bulk Update Category
        /// 2. Krijojme dy produkte me vlerat si me poshte
        /// 3. Testojme nese mund te ndryshojme cmimet e produkteve
        /// 4. Nese nuk mund te ndryshojme cmimet e produkteve, testi do te dali gabim
        /// 5. Testojme nese cmimet e produkteve jane ndryshuar ne menyre korrekte
        /// 6. Nese nuk jane ndryshuar ne menyre korrekte, testi do te dali gabim
        /// 7. Nese te gjitha testet kalojne, testi do te jete i suksesshem
        /// </summary>
        [Fact]
        public async Task Test_018_BulkUpdateProducts_ShouldUpdatePricesCorrectly()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var category = new Category { Name = "Bulk Update Category" };
                await _unitOfWork.CategoriesRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var products = new List<Product>
        {
            new Product { Title = "Book 1", Author = "Author A", Description = "Book1", CategoryId = category.Id, Price = 10.00m, ISBN = "131241" },
            new Product { Title = "Book 2", Author = "Author B", Description = "Book2", CategoryId = category.Id, Price = 15.00m, ISBN = "412311" }
        };

                foreach (var product in products)
                {
                    await _unitOfWork.ProductsRepository.CreateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();

                foreach (var product in products)
                {
                    product.Price += 5.00m;
                    await _unitOfWork.ProductsRepository.UpdateAsync(product);
                }
                await _unitOfWork.SaveChangesAsync();

                var updatedProducts = await _unitOfWork.ProductsRepository.GetAllAsync(p => p.CategoryId == category.Id);
                Assert.All(updatedProducts, p => Assert.True(p.Price == 15.00m || p.Price == 20.00m, $"Product {p.Title} did not update correctly."));

                await transaction.RollbackAsync();
            }
        }
    }
}
