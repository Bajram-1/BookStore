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
    public class BaseRepository<TEntity, TKey>(ApplicationDbContext applicationDbContext) : IBaseRepository<TEntity, TKey> where TEntity : BaseEntityWithKey<TKey>
    {
        protected DbSet<TEntity> _dbSet => applicationDbContext.Set<TEntity>();
        protected DbSet<TEntity> _nonTrackingDbSet => applicationDbContext.Set<TEntity>();

        public async Task CreateAsync(TEntity entity)
        {
           await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entity)
        {
            foreach (var orderDetail in entity)
            {
                var existingEntity = await _dbSet.FindAsync(orderDetail.Id);
                if (existingEntity != null)
                {
                    applicationDbContext.Entry(existingEntity).State = EntityState.Deleted;
                }
                else
                {
                    _dbSet.Attach(orderDetail);
                    _dbSet.Remove(orderDetail);
                }
            }
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<TEntity> query;
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
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();
        }
    }
}
