using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DAL.Repositories
{
    public class CategoriesRepository(ApplicationDbContext applicationDbContext) : BaseRepository<Category, int>(applicationDbContext), ICategoriesRepository
    {
        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            _dbSet.Remove(category);
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ?? throw new Exception("Category not found");
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<int> GetActualDisplayOrderAsync()
        {
            return await _dbSet.MaxAsync(c => (int?)c.DisplayOrder) ?? 0;
        }
    }
}