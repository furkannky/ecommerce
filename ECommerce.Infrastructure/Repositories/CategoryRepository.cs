using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
// using ECommerce.Infrastructure.Persistence; // Bu satırı kaldırın veya aşağıdakiyle değiştirin
using ECommerce.Infrastructure.Persistence.Context; // Bu namespace'i kullanmalısınız, çünkü ApplicationDbContext burada tanımlı.

namespace ECommerce.Infrastructure.Repositories
{
    // ICategoryRepository arayüzünü implemente eder ve GenericRepository<Category>'den miras alır
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        // Base sınıfa DbContext'i gönderiyoruz
        // BURADAKİ DÜZELTME: ApplicationDbContextFactory yerine ApplicationDbContext olmalı
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Kategoriye özel ek repository operasyonları buraya eklenebilir.
    }
}