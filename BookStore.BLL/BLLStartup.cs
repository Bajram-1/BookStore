using BookStore.BLL.IServices;
using BookStore.BLL.Services;
using BookStore.BLL.Services.Hosted;
using BookStore.BLL.Services.Singletons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL
{
    public static class BLLStartup
    {
        public static void RegisterBLLServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ILoggerService, LoggerService>();

            services.AddHostedService<AuditLogsHostedService>();

            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IInternalAuditLogService, AuditLogService>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IApplicationUsersService, ApplicationUsersService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ICompaniesService, CompaniesService>();
            services.AddScoped<IOrderDetailsService, OrderDetailsService>();
            services.AddScoped<IOrderHeadersService, OrderHeadersService>();
            services.AddScoped<IProductImagesService, ProductImagesService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IShoppingCartsService, ShoppingCartsService>();
        }
    }
}