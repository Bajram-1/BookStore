using BookStore.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IAuditableEntityService
    {
        event OnEntityAdded OnEntityAdded;
        event OnEntityDeleted OnEntityDeleted;
        event OnEntityUpdated OnEntityUpdated;
    }
}
