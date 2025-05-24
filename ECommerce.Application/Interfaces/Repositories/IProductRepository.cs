using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories
{
    // IRepository<Product> arayüzünden tüm temel operasyonları miras alır
    public interface IProductRepository : IRepository<Product>
    {
        // Ürüne özel operasyonlar eklenebilir, şimdilik boş
    }
}