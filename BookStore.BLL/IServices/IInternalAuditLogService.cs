using BookStore.DAL.Entities;

namespace BookStore.BLL.IServices
{
    public interface IInternalAuditLogService : IAuditLogService
    {
        void Add(AuditLog auditLog);
    }
}