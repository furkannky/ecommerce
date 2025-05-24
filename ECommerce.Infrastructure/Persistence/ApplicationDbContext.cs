using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Domain.Common; // BaseEntity için bu using'i ekleyin

namespace ECommerce.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IdentityDbContext'in kendi model oluşturma mantığını çalıştırın. ZORUNLUDUR.
            base.OnModelCreating(modelBuilder);

            // ----------- RowVersion Eşzamanlılık Belirteci Konfigürasyonu -----------
            // BaseEntity'den miras alan tüm varlıklar için RowVersion'ı eşzamanlılık belirteci olarak ayarlar.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Yalnızca BaseEntity'den türeyen ve RowVersion özelliğine sahip olanlara uygula
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) &&
                    entityType.FindProperty("RowVersion") != null) // RowVersion özelliği var mı kontrol et
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion(); // Bu, EF Core'a bu alanı eşzamanlılık belirteci olarak kullanmasını söyler
                }
            }
            // ----------------------------------------------------------------------


            // ----------- Soft Delete Filtreleri (Query Filters) -----------
            // Eğer BaseEntity üzerinde global bir filtre uygulamıyorsanız, her bir entity için ayrı ayrı eklemeniz gerekir.
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ProductImage>().HasQueryFilter(pi => !pi.IsDeleted);
            // Diğer entity'leriniz için de IsDeleted filtresi varsa buraya ekleyin.
            // -------------------------------------------------------------


            // ----------- İlişkiler (Zaten Mevcutsa Dokunmayın) -----------
            // Product ve Category arasındaki ilişki
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Product ve ProductImage arasındaki ilişki
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ürün silindiğinde resimleri de sil
            // -------------------------------------------------------------


            // ----------- Seed Data (Başlangıç Verisi) (Zaten Mevcutsa Dokunmayın) -----------
            // Kategoriler
            var electronicsCategory = new Category { Id = Guid.Parse("A1B2C3D4-E5F6-0001-0000-000000000001"), Name = "Elektronik", Description = "Akıllı cihazlar ve bileşenler", CreatedDate = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) };
            var clothingCategory = new Category { Id = Guid.Parse("A1B2D3D4-E5F6-0001-0000-000000000002"), Name = "Giyim", Description = "Her mevsim için kıyafetler", CreatedDate = new DateTime(2023, 1, 1, 10, 1, 0, DateTimeKind.Utc) };
            var homeAppliancesCategory = new Category { Id = Guid.Parse("A1B2C3D4-E5F6-0001-0000-000000000003"), Name = "Ev Aletleri", Description = "Mutfak ve ev için küçük ev aletleri", CreatedDate = new DateTime(2023, 1, 1, 10, 2, 0, DateTimeKind.Utc) };
            var booksCategory = new Category { Id = Guid.Parse("A1B2C3D4-E5F6-0001-0000-000000000004"), Name = "Kitaplar", Description = "Farklı türlerde kitaplar", CreatedDate = new DateTime(2023, 1, 1, 10, 3, 0, DateTimeKind.Utc) };

            modelBuilder.Entity<Category>().HasData(
                electronicsCategory,
                clothingCategory,
                homeAppliancesCategory,
                booksCategory
            );

            // Ürünler
            var products = new List<Product>();

            // Elektronik Ürünler
            var phone1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000001"), Name = "Akıllı Telefon X", Description = "Son model akıllı telefon", Price = 8999.99m, Stock = 120, CategoryId = electronicsCategory.Id, CreatedDate = new DateTime(2023, 1, 15, 12, 0, 0, DateTimeKind.Utc) };
            var laptop1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000002"), Name = "Gaming Laptop Pro", Description = "Yüksek performanslı oyun bilgisayarı", Price = 18500.00m, Stock = 50, CategoryId = electronicsCategory.Id, CreatedDate = new DateTime(2023, 1, 15, 12, 1, 0, DateTimeKind.Utc) };
            var tablet1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000003"), Name = "Tablet Go", Description = "Hafif ve taşınabilir tablet", Price = 2499.99m, Stock = 200, CategoryId = electronicsCategory.Id, CreatedDate = new DateTime(2023, 1, 15, 12, 2, 0, DateTimeKind.Utc) };
            var headset1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000004"), Name = "Kablosuz Kulaklık", Description = "Gürültü engelleme özellikli kulaklık", Price = 799.00m, Stock = 300, CategoryId = electronicsCategory.Id, CreatedDate = new DateTime(2023, 1, 15, 12, 3, 0, DateTimeKind.Utc) };
            products.AddRange(new[] { phone1, laptop1, tablet1, headset1 });

            // Giyim Ürünleri
            var tshirt1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000005"), Name = "Erkek T-Shirt Pamuklu", Description = "Rahat kesim, %100 pamuk", Price = 129.90m, Stock = 450, CategoryId = clothingCategory.Id, CreatedDate = new DateTime(2023, 2, 1, 9, 0, 0, DateTimeKind.Utc) };
            var jeans1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000006"), Name = "Kadın Jean Pantolon", Description = "Yüksek bel, dar paça jean", Price = 349.50m, Stock = 280, CategoryId = clothingCategory.Id, CreatedDate = new DateTime(2023, 2, 1, 9, 1, 0, DateTimeKind.Utc) };
            var jacket1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000007"), Name = "Unisex Yağmurluk", Description = "Su geçirmez hafif yağmurluk", Price = 599.00m, Stock = 100, CategoryId = clothingCategory.Id, CreatedDate = new DateTime(2023, 2, 1, 9, 2, 0, DateTimeKind.Utc) };
            products.AddRange(new[] { tshirt1, jeans1, jacket1 });

            // Ev Aletleri
            var blender1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000008"), Name = "Akıllı Blender", Description = "Çok fonksiyonlu mutfak blenderı", Price = 1200.00m, Stock = 70, CategoryId = homeAppliancesCategory.Id, CreatedDate = new DateTime(2023, 3, 5, 14, 0, 0, DateTimeKind.Utc) };
            var coffeeMachine1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000009"), Name = "Otomatik Kahve Makinesi", Description = "Çekirdekten öğütme özellikli", Price = 2800.00m, Stock = 40, CategoryId = homeAppliancesCategory.Id, CreatedDate = new DateTime(2023, 3, 5, 14, 1, 0, DateTimeKind.Utc) };
            products.AddRange(new[] { blender1, coffeeMachine1 });

            // Kitaplar
            var novel1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000010"), Name = "Bilim Kurgu Destanı", Description = "Yılın en çok okunan bilim kurgu romanı", Price = 85.00m, Stock = 600, CategoryId = booksCategory.Id, CreatedDate = new DateTime(2023, 4, 10, 16, 0, 0, DateTimeKind.Utc) };
            var historyBook1 = new Product { Id = Guid.Parse("B1C2D3E4-F5A6-0002-0000-000000000011"), Name = "Antik Uygarlıklar Tarihi", Description = "Detaylı ve resimli tarih kitabı", Price = 150.00m, Stock = 250, CategoryId = booksCategory.Id, CreatedDate = new DateTime(2023, 4, 10, 16, 1, 0, DateTimeKind.Utc) };
            products.AddRange(new[] { novel1, historyBook1 });

            modelBuilder.Entity<Product>().HasData(products);

            // Ürün Resimleri (Her ürün için bir veya iki resim ekliyoruz)
            var productImages = new List<ProductImage>();

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000001"), ProductId = phone1.Id, ImagePath = "/uploads/products/telefon.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 1, 15, 12, 0, 5, DateTimeKind.Utc) });
            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000002"), ProductId = phone1.Id, ImagePath = "/uploads/products/telefon_detay.jpg", DisplayOrder = 2, CreatedDate = new DateTime(2023, 1, 15, 12, 0, 10, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000003"), ProductId = laptop1.Id, ImagePath = "/uploads/products/laptop.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 1, 15, 12, 1, 5, DateTimeKind.Utc) });
            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000004"), ProductId = laptop1.Id, ImagePath = "/uploads/products/laptop_klavye.jpg", DisplayOrder = 2, CreatedDate = new DateTime(2023, 1, 15, 12, 1, 10, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000005"), ProductId = tablet1.Id, ImagePath = "/uploads/products/tablet.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 1, 15, 12, 2, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000006"), ProductId = headset1.Id, ImagePath = "/uploads/products/kulaklik.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 1, 15, 12, 3, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000007"), ProductId = tshirt1.Id, ImagePath = "/uploads/products/tshirt.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 2, 1, 9, 0, 5, DateTimeKind.Utc) });
            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000008"), ProductId = tshirt1.Id, ImagePath = "/uploads/products/tshirt_model.jpg", DisplayOrder = 2, CreatedDate = new DateTime(2023, 2, 1, 9, 0, 10, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000009"), ProductId = jeans1.Id, ImagePath = "/uploads/products/jean.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 2, 1, 9, 1, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000010"), ProductId = jacket1.Id, ImagePath = "/uploads/products/yagmurluk.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 2, 1, 9, 2, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000011"), ProductId = blender1.Id, ImagePath = "/uploads/products/blender.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 3, 5, 14, 0, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000012"), ProductId = coffeeMachine1.Id, ImagePath = "/uploads/products/kahve_makinesi.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 3, 5, 14, 1, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000013"), ProductId = novel1.Id, ImagePath = "/uploads/products/bilim_kurgu_kitap.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 4, 10, 16, 0, 5, DateTimeKind.Utc) });

            productImages.Add(new ProductImage { Id = Guid.Parse("C1D2E3F4-A5B6-0003-0000-000000000014"), ProductId = historyBook1.Id, ImagePath = "/uploads/products/tarih_kitap.jpg", DisplayOrder = 1, CreatedDate = new DateTime(2023, 4, 10, 16, 1, 5, DateTimeKind.Utc) });

            modelBuilder.Entity<ProductImage>().HasData(productImages);
            // -------------------------------------------------------------------
        }

        // Soft Delete ve CreatedDate/UpdatedDate takibi için SaveChanges metodunu override ediyoruz.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>()) // Tüm BaseEntity türevlerini kontrol et
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        // BaseEntity constructor'ında zaten Id ve IsDeleted set edildiği için burada tekrar etmeye gerek yok.
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        // Soft delete: Varlığı silmek yerine IsDeleted bayrağını true yapar ve durumu Modified olarak ayarlar.
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}