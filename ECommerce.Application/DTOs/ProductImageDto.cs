using System;

namespace ECommerce.Application.DTOs
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMainImage { get; set; }
        public object ProductId { get; set; }
    }
}