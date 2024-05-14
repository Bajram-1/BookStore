using BookStore.DAL.Entities;
using BookStore.DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Repositories
{
    public class AuditLogsRepository(ApplicationDbContext context) : BaseRepository<AuditLog, long>(context), IAuditLogsRepository
    {
    }
}
