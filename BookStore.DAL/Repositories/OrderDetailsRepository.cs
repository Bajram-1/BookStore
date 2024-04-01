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
    public class OrderDetailsRepository(ApplicationDbContext applicationDbContext) : IOrderDetailsRepository
    {
        private readonly DbSet<OrderDetail> _dbSet = applicationDbContext.Set<OrderDetail>();

        public void Update(OrderDetail orderDetail)
        {
            _dbSet.Update(orderDetail);
        }

        public void Add(OrderDetail orderDetail)
        {
            _dbSet.Add(orderDetail);
        }

        public OrderDetail Get(Expression<Func<OrderDetail, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<OrderDetail> query;
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

        public IEnumerable<OrderDetail> GetAll(Expression<Func<OrderDetail, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<OrderDetail> query = _dbSet;
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

        public void Remove(OrderDetail orderDetail)
        {
            _dbSet.Remove(orderDetail);
        }

        public void RemoveRange(IEnumerable<OrderDetail> orderDetails)
        {
            _dbSet.RemoveRange(orderDetails);
        }
    }
}