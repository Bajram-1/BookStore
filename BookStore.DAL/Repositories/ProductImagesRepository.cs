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
    public class ProductImagesRepository(ApplicationDbContext applicationDbContext) : BaseRepository<ProductImage, int>(applicationDbContext), IProductImagesRepository
    {
        public async Task<IEnumerable<ProductImage>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}