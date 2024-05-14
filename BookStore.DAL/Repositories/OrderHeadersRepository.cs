using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class OrderHeadersRepository(ApplicationDbContext applicationDbContext) : BaseRepository<OrderHeader, int>(applicationDbContext), IOrderHeadersRepository
    {
        public async Task UpdateAsync(OrderHeader obj)
        {
            _dbSet.Update(obj);
            await Task.CompletedTask;
        }

        public async Task UpdateStatusAsync(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public async Task<OrderHeader> GetItemAsync(int orderId, string includeProperties)
        {
            IQueryable<OrderHeader> query = applicationDbContext.Set<OrderHeader>();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(x => x.Id == orderId);
        }
    }
}