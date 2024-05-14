using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface IServiceManager
    {
        T ExecuteTransaction<T>(Func<T> func);
    }
}
