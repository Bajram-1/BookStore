using BookStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.IRepositories
{
    public interface IOrderHeadersRepository
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        IEnumerable<OrderHeader> GetAll(Expression<Func<OrderHeader, bool>>? filter = null, string? includeProperties = null);
        OrderHeader Get(Expression<Func<OrderHeader, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(OrderHeader orderHeader);
        void Remove(OrderHeader orderHeader);
        void RemoveRange(IEnumerable<OrderHeader> orderHeaders);
        OrderHeader GetItem(int orderId, string includeProperties);
    }
}
