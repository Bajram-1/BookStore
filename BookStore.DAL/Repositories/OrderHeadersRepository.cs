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
    public class OrderHeadersRepository(ApplicationDbContext applicationDbContext) : IOrderHeadersRepository
    {
        private readonly DbSet<OrderHeader> _dbSet = applicationDbContext.Set<OrderHeader>();

        public void Update(OrderHeader obj)
        {
            _dbSet.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _dbSet.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _dbSet.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }

        public void Add(OrderHeader orderHeader)
        {
            _dbSet.Add(orderHeader);
        }

        public OrderHeader Get(Expression<Func<OrderHeader, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<OrderHeader> query;
            if (tracked)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<OrderHeader> GetAll(Expression<Func<OrderHeader, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<OrderHeader> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(OrderHeader orderHeader)
        {
            _dbSet.Remove(orderHeader);
        }

        public void RemoveRange(IEnumerable<OrderHeader> orderHeaders)
        {
            _dbSet.RemoveRange(orderHeaders);
        }

        public OrderHeader GetItem(int orderId, string includeProperties)
        {
            IQueryable<OrderHeader> query = applicationDbContext.Set<OrderHeader>();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault(x => x.Id == orderId);
        }
    }
}