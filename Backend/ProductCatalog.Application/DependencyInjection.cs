using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Services;

namespace ProductCatalog.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
