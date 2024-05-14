using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.IServices
{
    public interface ILoggerService
    {
        void LogError(string message);
        void LogError(Exception exception);
        void Log(string logType, string logData);
    }
}
