using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IUnitOfWork
    {
        IApplicationUsersRepository ApplicationUser { get; }
        ICompaniesRepository Companies { get; }
        IProductsRepository Products { get; }

        void SaveChanges();
    }
}