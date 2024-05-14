using BookStore.BLL.IServices;
using BookStore.DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace BookStore.BLL.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private static readonly object _lock = new object();
        public ServiceManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public T ExecuteTransaction<T>(Func<T> func)
        {
            lock (_lock)
            {
                try
                {
                    return _unitOfWork.ExecuteTransaction(func);
                }
                catch
                {
                    throw;
                }
            }
        }
    }

}
