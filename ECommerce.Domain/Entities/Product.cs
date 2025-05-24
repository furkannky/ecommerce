using ECommerce.Domain.Common;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // İlişkiler
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } // Navigasyon özelliği

        // Bir ürünün birden fazla resmi olabilir
        public ICollection<ProductImage> ProductImages { get; set; }

        public Product()
        {
            ProductImages = new HashSet<ProductImage>(); // Yeni bir liste örneği oluştur
        }
    }
}