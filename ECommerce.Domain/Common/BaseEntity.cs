// ECommerce.Domain\Common\BaseEntity.cs

using System;

namespace ECommerce.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } // Her varlığın benzersiz bir kimliği olacak

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi, UTC olarak varsayılan değerle
        public DateTime? UpdatedDate { get; set; } // Güncelleme tarihi, null olabilir
        public bool IsDeleted { get; set; } = false; // Soft delete için

        // İyimser eşzamanlılık (Optimistic Concurrency) kontrolü için bu alanı ekleyin.
        // EF Core bu alanı otomatik olarak güncelleyecektir.
        public byte[] RowVersion { get; set; }

        // Yapıcı metot (constructor)
        protected BaseEntity()
        {
            Id = Guid.NewGuid(); // Yeni bir GUID otomatik olarak oluşturulur
            CreatedDate = DateTime.UtcNow; // Oluşturulma tarihi otomatik set edilir
            IsDeleted = false;
        }
    }
}