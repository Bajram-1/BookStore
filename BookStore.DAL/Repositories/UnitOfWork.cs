using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookStore.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUsersRepository ApplicationUserRepository { get; private set; }
        public ICategoriesRepository CategoriesRepository { get; private set; }
        public ICompaniesRepository CompaniesRepository { get; private set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; private set; }
        public IOrderHeadersRepository OrderHeadersRepository { get; private set; }
        public IProductImagesRepository ProductImagesRepository { get; private set; }
        public IProductsRepository ProductsRepository { get; private set; }
        public IShoppingCartsRepository ShoppingCartsRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUserRepository = new ApplicationUsersRepository(_db);
            CategoriesRepository = new CategoriesRepository(_db);
            CompaniesRepository = new CompaniesRepository(_db);
            OrderDetailsRepository = new OrderDetailsRepository(_db);
            OrderHeadersRepository = new OrderHeadersRepository(_db);
            ProductImagesRepository = new ProductImagesRepository(_db);
            ProductsRepository = new ProductsRepository(_db);
            ShoppingCartsRepository = new ShoppingCartsRepository(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public T ExecuteTransaction<T>(Func<T> func)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var result = func();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }
    }
}