using BookStore.BLL.IServices;
using BookStore.DAL.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.Hosted
{
    public class AuditLogsHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public AuditLogsHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void AddLog(AuditLog log)
        {
            var scope = _serviceProvider.CreateScope();
            var auditLogService = scope.ServiceProvider.GetRequiredService<IInternalAuditLogService>();
            auditLogService.Add(log);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ProductsService.OnEntityAdded += AddLog;
            ProductsService.OnEntityDeleted += AddLog;
            ProductsService.OnEntityUpdated += AddLog;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ProductsService.OnEntityAdded -= AddLog;
            ProductsService.OnEntityDeleted -= AddLog;
            ProductsService.OnEntityUpdated -= AddLog;
        }
    }
}

