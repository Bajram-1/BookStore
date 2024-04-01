using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class ShoppingCartsRepository(ApplicationDbContext applicationDbContext) : IShoppingCartsRepository
    {
        private readonly DbSet<ShoppingCart> _dbSet = applicationDbContext.Set<ShoppingCart>();

        public void Update(ShoppingCart shoppingCart)
        {
            _dbSet.Update(shoppingCart);
        }

        public void Add(ShoppingCart shoppingCart)
        {
            _dbSet.Add(shoppingCart);
        }

        public ShoppingCart Get(Expression<Func<ShoppingCart, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<ShoppingCart> query;
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

        public IEnumerable<ShoppingCart> GetAll(Expression<Func<ShoppingCart, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<ShoppingCart> query = _dbSet;
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

        public void Remove(ShoppingCart shoppingCart)
        {
            _dbSet.Remove(shoppingCart);
        }

        public void RemoveRange(IEnumerable<ShoppingCart> shoppingCarts)
        {
            _dbSet.RemoveRange(shoppingCarts);
        }

        public IEnumerable<ShoppingCart> GetCartItems(string userId)
        {
            return _dbSet.Where(item => item.ApplicationUserId == userId).ToList();
        }
    }
}