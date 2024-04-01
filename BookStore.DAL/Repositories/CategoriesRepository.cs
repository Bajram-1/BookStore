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
    public class CategoriesRepository(ApplicationDbContext applicationDbContext) : ICategoriesRepository
    {
        private readonly DbSet<Category> _dbSet = applicationDbContext.Set<Category>();

        public void Create(Category category)
        {
            _dbSet.Add(category);
        }

        public void Delete(int id)
        {
            var category = GetById(id);
            _dbSet.Remove(category);
        }

        public Category GetById(int id)
        {
            return _dbSet.Find(id) ?? throw new Exception("Category not found");
        }

        public IEnumerable<Category> GetAll()
        {
            return _dbSet.ToList();
        }
    }
}