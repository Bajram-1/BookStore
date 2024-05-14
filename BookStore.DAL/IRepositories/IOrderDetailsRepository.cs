using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IOrderDetailsRepository : IBaseRepository<OrderDetail, int>
    {
        Task<OrderDetail> GetAsync(Expression<Func<OrderDetail, bool>> filter, string? includeProperties = null, bool tracked = false);
    }
}