using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IUnitOfWork
    {
        IApplicationUsersRepository ApplicationUserRepository { get; }
        ICategoriesRepository CategoriesRepository { get; }
        ICompaniesRepository CompaniesRepository { get; }
        IOrderDetailsRepository OrderDetailsRepository { get; }
        IOrderHeadersRepository OrderHeadersRepository { get; }
        IProductImagesRepository ProductImagesRepository { get; }
        IProductsRepository ProductsRepository { get; }
        IShoppingCartsRepository ShoppingCartsRepository { get; }

        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        T ExecuteTransaction<T>(Func<T> func);
    }
}