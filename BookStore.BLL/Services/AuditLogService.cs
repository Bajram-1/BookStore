using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public delegate void OnEntityAdded(AuditLog log);
    public delegate void OnEntityDeleted(AuditLog log);
    public delegate void OnEntityUpdated(AuditLog log);

    public class AuditLogService(IAuditLogsRepository auditLogsRepository, IUnitOfWork unitOfWork) : IAuditLogService, IInternalAuditLogService
    {
        private readonly IAuditLogsRepository _auditLogsRepository = auditLogsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async void Add(AuditLog auditLog)
        {
            _auditLogsRepository.CreateAsync(auditLog).Wait();
            await _unitOfWork.SaveChangesAsync();
        }

        public IEnumerable<AuditLog> GetAuditLogs()
        {
            return _auditLogsRepository.GetAllAsync(null, null).Result;
        }
    }
}
