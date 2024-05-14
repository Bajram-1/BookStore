using BookStore.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Entities
{
    public class AuditLog : BaseEntityWithKey<long>
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public AuditLogType LogType { get; set; }
        public string Details { get; set; }
        public DateTime CreatedOn { get; }
    }
}
