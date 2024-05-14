using BookStore.BLL.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.Singletons
{
    public class LoggerService : ILoggerService
    {
        private static object _lock = new object();
        private string _logDir;

        public LoggerService(IConfiguration configuration)
        {
            _logDir = configuration["LogsDirectory"];
            if (!System.IO.Directory.Exists(_logDir))
            {
                System.IO.Directory.CreateDirectory(_logDir);
            }
        }

        public void LogError(string message)
        {
            var logData = $"Error: {message}";
            Log("Error", logData);
        }

        public void LogError(Exception exception)
        {
            var logData = $"{exception.Message} {exception.StackTrace}";
            LogError(logData);
        }

        public void Log(string logType, string logData)
        {
            lock (_lock)
            {
                var logFileName = $"{_logDir}/{logType}-{DateTime.Now.ToString("yyyy-MM-dd")}.log";
                var logMessage = $"" +
                    $"{Environment.NewLine}" +
                    $"==========" +
                    $"{Environment.NewLine}" +
                    $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}" +
                    $"{Environment.NewLine}" +
                    $"{logData}" +
                    $"{Environment.NewLine}" +
                    $"==========" +
                    $"{Environment.NewLine}" +
                    $"";
                System.IO.File.AppendAllText(logFileName, logMessage);
            }
        }
    }
}