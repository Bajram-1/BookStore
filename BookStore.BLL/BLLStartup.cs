using BookStore.BLL.IServices;
using BookStore.BLL.Services;
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