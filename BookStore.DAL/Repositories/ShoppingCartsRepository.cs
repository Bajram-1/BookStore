using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookStore.DAL.Repositories
{
    public class ShoppingCartsRepository(ApplicationDbContext applicationDbContext) : BaseRepository<ShoppingCart, int>(applicationDbContext), IShoppingCartsRepository
    {
        public async Task UpdateAsync(ShoppingCart shoppingCart)
        {
            _dbSet.Update(shoppingCart);
            await Task.CompletedTask;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _dbSet
                .Where(item => item.ApplicationUserId == userId)
                .CountAsync();
        }

        public async Task<List<DAL.Entities.ShoppingCart>> GetAllAsync(Expression<Func<DAL.Entities.ShoppingCart, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }
    }
}