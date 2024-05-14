using BookStore.DAL.Entities;

namespace BookStore.DAL.IRepositories
{
    public interface IAuditLogsRepository : IBaseRepository<AuditLog, long>
    {
    }
}