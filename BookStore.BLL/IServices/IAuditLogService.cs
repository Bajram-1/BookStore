using BookStore.DAL.Entities;

namespace BookStore.BLL.IServices
{
    public interface IAuditLogService
    {
        IEnumerable<AuditLog> GetAuditLogs();
    }
}