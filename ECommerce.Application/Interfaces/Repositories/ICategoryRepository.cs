using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories
{
    // IRepository<Category> arayüzünden tüm temel operasyonları miras alır
    public interface ICategoryRepository : IRepository<Category>
    {
        // Kategoriye özel operasyonlar eklenebilir, şimdilik boş
    }
}