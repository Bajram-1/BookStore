using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IOrderDetailsRepository
    {
        void Update(OrderDetail orderDetail);
        IEnumerable<OrderDetail> GetAll(Expression<Func<OrderDetail, bool>>? filter = null, string? includeProperties = null);
        OrderDetail Get(Expression<Func<OrderDetail, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(OrderDetail orderDetail);
        void Remove(OrderDetail orderDetail);
        void RemoveRange(IEnumerable<OrderDetail> orderDetails);
    }
}