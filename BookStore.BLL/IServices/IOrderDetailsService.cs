using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IOrderDetailsService
    {
        DTO.OrderDetail GetOrderDetailById(int id);
        void AddOrderDetail(DAL.Entities.OrderDetail orderDetail);
        void UpdateOrderDetail(OrderDetailsAddEditRequestModel orderDetail);
        void DeleteOrderDetail(int id);
        void RemoveRange();
        DTO.OrderDetail GetAll(Func<DTO.OrderDetail, bool> value, string includeProperties);
    }
}
