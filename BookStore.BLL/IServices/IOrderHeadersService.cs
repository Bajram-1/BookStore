using BookStore.BLL.DTO;
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
    public interface IOrderHeadersService
    {
        Task<IEnumerable<DTO.OrderHeader>> GetAllAsync(Expression<Func<DAL.Entities.OrderHeader, bool>>? filter = null, string includeProperties = null);
        Task<BLL.DTO.OrderHeader> AddAsync(BLL.DTO.OrderHeader model);
        Task UpdateAsync(BLL.DTO.OrderHeader model);
        Task UpdateStatusAsync(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        Task<DTO.OrderHeader> GetAsync(Expression<Func<DAL.Entities.OrderHeader, bool>> predicate, string includeProperties = "ApplicationUser");
        Task<DTO.OrderHeader> GetOrderHeaderAsync(Expression<Func<DAL.Entities.OrderHeader, bool>> predicate);
    }
}
