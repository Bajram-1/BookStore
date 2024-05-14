using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : BaseEntityWithKey<TKey>
    {
        Task CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task RemoveRangeAsync(IEnumerable<TEntity> entity);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null, bool tracked = false);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, string? includeProperties = null);
    }
}