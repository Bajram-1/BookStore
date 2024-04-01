using BookStore.BLL.DTO;
using BookStore.BLL.DTO.Requests;
using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IOrderHeadersService
    {
        IEnumerable<DTO.OrderHeader> GetAllOrderDetails();
        DTO.OrderHeader GetOrderDetailById(int id);
        void Add(OrderHeaderAddEditRequestModel model);
        void Update(OrderHeaderAddEditRequestModel model);
        void Delete(int id);
        void RemoveRange(IEnumerable<DTO.OrderHeader> orderHeaders);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        void UpdateShippingInfo(int orderId, string carrier, string trackingNumber);
        DTO.OrderHeader Get(Func<DTO.OrderHeader, bool> value, string includeProperties);
    }
}
