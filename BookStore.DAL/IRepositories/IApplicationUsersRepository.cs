using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface IApplicationUsersRepository
    {
        Task UpdateAsync(ApplicationUser applicationUser);
        Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> filter, string includeProperties = null, bool tracked = false);
        Task<IEnumerable<ApplicationUser>> GetAllAsync(Expression<Func<ApplicationUser, bool>> filter = null, string includeProperties = null);
        Task<ApplicationUser> GetByIdAsync(string userId);
        Task<ApplicationUser> GetUserWithCompanyAsync(string userId);
    }
}