using ECommerce.Domain.Common;
using System;

namespace ECommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImagePath { get; set; } // Resmin dosya yolu veya URL'si
        public int DisplayOrder { get; set; } // Görüntülenme sırası

        // İlişki: Hangi ürüne ait olduğu
        public Guid ProductId { get; set; }
        public Product Product { get; set; } // Navigasyon özelliği
    }
}