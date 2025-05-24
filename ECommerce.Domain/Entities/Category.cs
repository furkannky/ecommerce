using ECommerce.Domain.Common;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // İlişkiler: Bir kategoriye ait birden fazla ürün olabilir
        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>(); // Yeni bir liste örneği oluştur
        }
    }
}