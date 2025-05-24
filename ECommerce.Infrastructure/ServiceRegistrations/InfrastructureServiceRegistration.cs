// ECommerce.Infrastructure/ServiceRegistrations/InfrastructureServiceRegistration.cs

using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services; // IProductService ve ICategoryService, ICartService için!
using ECommerce.Application.Interfaces.Storage;  // IFileStorageService için
using ECommerce.Infrastructure.Managers;         // ProductManager, CategoryManager, ve **CartManager** için!
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;         // LocalFileStorageService için
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductManager = ECommerce.Infrastructure.Services.ProductManager; // Bu satırda bir karışıklık olabilir, emin değilim ama şimdilik kalsın.

namespace ECommerce.Infrastructure.ServiceRegistrations
{
    public static class InfrastructureServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext'i bağımlılık enjeksiyon sistemine ekliyoruz
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repository'leri DI konteynerine ekliyoruz
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Servis implementasyonlarını DI'a kaydediyoruz
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

            // **!!!! Eklenecek Satır Burası !!!!**
            services.AddScoped<ICartService, CartManager>();

            // HttpContextAccessor'ı da eklemeniz gerekiyor, çünkü CartManager bunu kullanıyor.
            services.AddHttpContextAccessor();


            // Dosya depolama servisini DI'a kaydediyoruz
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // Diğer altyapı servisleri (e-posta, loglama, vs.) buraya eklenebilir
        }
    }
}