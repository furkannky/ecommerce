using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
// using ECommerce.Infrastructure.Persistence; // Bu satırı kaldırın veya aşağıdakiyle değiştirin
using ECommerce.Infrastructure.Persistence.Context; // Bu namespace'i kullanmalısınız, çünkü ApplicationDbContext burada tanımlı.

namespace ECommerce.Infrastructure.Repositories
{
    // IProductRepository arayüzünü implemente eder ve GenericRepository<Product>'tan miras alır
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        // Base sınıfa DbContext'i gönderiyoruz
        // BURADAKİ DÜZELTME: ApplicationDbContextFactory yerine ApplicationDbContext olmalı
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Ürüne özel ek repository operasyonları buraya eklenebilir.
        // Şimdilik sadece temel CRUD operasyonları GenericRepository tarafından karşılanıyor.
    }
}