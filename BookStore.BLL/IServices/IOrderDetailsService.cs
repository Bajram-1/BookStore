using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IOrderDetailsService
    {
        Task AddOrderDetailAsync(BLL.DTO.OrderDetail orderDetail, int orderHeaderId);
        Task<IEnumerable<DTO.OrderDetail>> GetAllByOrderHeaderIdAsync(Expression<Func<DAL.Entities.OrderDetail, bool>> predicate, string includeProperties = "Product");
    }
}
