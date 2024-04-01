using BookStore.DAL.Entities;
using System.Linq.Expressions;

namespace BookStore.DAL.IRepositories
{
    public interface IApplicationUsersRepository
    {
        public void Update(ApplicationUser applicationUser);
        IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter = null, string? includeProperties = null);
        ApplicationUser Get(Expression<Func<ApplicationUser, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(ApplicationUser applicationUser);
        void Remove(ApplicationUser applicationUser);
        void RemoveRange(IEnumerable<ApplicationUser> applicationUsers);
        ApplicationUser GetById(string id);

        ApplicationUser GetUserWithCompany(string userId);
    }
}