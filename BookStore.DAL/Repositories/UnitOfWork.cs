using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
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
        private ApplicationDbContext _db;
        public IApplicationUsersRepository ApplicationUser { get; private set; }
        public ICompaniesRepository Companies { get; private set; }
        public IProductsRepository Products { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;          
            ApplicationUser = new ApplicationUsersRepository(_db);
            Companies = new CompaniesRepository(_db);
            Products = new ProductsRepository(_db);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}