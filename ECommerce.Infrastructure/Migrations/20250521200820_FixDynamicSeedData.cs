using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-0001-0000-000000000001"), new DateTime(2023, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Akıllı cihazlar ve bileşenler", false, "Elektronik", null },
                    { new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 1, 1, 10, 1, 0, 0, DateTimeKind.Utc), "Her mevsim için kıyafetler", false, "Giyim", null },
                    { new Guid("a1b2c3d4-e5f6-0001-0000-000000000003"), new DateTime(2023, 1, 1, 10, 2, 0, 0, DateTimeKind.Utc), "Mutfak ve ev için küçük ev aletleri", false, "Ev Aletleri", null },
                    { new Guid("a1b2c3d4-e5f6-0001-0000-000000000004"), new DateTime(2023, 1, 1, 10, 3, 0, 0, DateTimeKind.Utc), "Farklı türlerde kitaplar", false, "Kitaplar", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "IsDeleted", "Name", "Price", "Stock", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000001"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000001"), new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Son model akıllı telefon", false, "Akıllı Telefon X", 8999.99m, 120, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000002"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000001"), new DateTime(2023, 1, 15, 12, 1, 0, 0, DateTimeKind.Utc), "Yüksek performanslı oyun bilgisayarı", false, "Gaming Laptop Pro", 18500.00m, 50, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000003"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000001"), new DateTime(2023, 1, 15, 12, 2, 0, 0, DateTimeKind.Utc), "Hafif ve taşınabilir tablet", false, "Tablet Go", 2499.99m, 200, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000004"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000001"), new DateTime(2023, 1, 15, 12, 3, 0, 0, DateTimeKind.Utc), "Gürültü engelleme özellikli kulaklık", false, "Kablosuz Kulaklık", 799.00m, 300, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000005"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Rahat kesim, %100 pamuk", false, "Erkek T-Shirt Pamuklu", 129.90m, 450, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000006"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 2, 1, 9, 1, 0, 0, DateTimeKind.Utc), "Yüksek bel, dar paça jean", false, "Kadın Jean Pantolon", 349.50m, 280, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000007"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 2, 1, 9, 2, 0, 0, DateTimeKind.Utc), "Su geçirmez hafif yağmurluk", false, "Unisex Yağmurluk", 599.00m, 100, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000008"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000003"), new DateTime(2023, 3, 5, 14, 0, 0, 0, DateTimeKind.Utc), "Çok fonksiyonlu mutfak blenderı", false, "Akıllı Blender", 1200.00m, 70, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000009"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000003"), new DateTime(2023, 3, 5, 14, 1, 0, 0, DateTimeKind.Utc), "Çekirdekten öğütme özellikli", false, "Otomatik Kahve Makinesi", 2800.00m, 40, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000010"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000004"), new DateTime(2023, 4, 10, 16, 0, 0, 0, DateTimeKind.Utc), "Yılın en çok okunan bilim kurgu romanı", false, "Bilim Kurgu Destanı", 85.00m, 600, null },
                    { new Guid("b1c2d3e4-f5a6-0002-0000-000000000011"), new Guid("a1b2c3d4-e5f6-0001-0000-000000000004"), new DateTime(2023, 4, 10, 16, 1, 0, 0, DateTimeKind.Utc), "Detaylı ve resimli tarih kitabı", false, "Antik Uygarlıklar Tarihi", 150.00m, 250, null }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "CreatedDate", "DisplayOrder", "ImagePath", "IsDeleted", "ProductId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000001"), new DateTime(2023, 1, 15, 12, 0, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/telefon.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000001"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000002"), new DateTime(2023, 1, 15, 12, 0, 10, 0, DateTimeKind.Utc), 2, "/uploads/products/telefon_detay.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000001"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000003"), new DateTime(2023, 1, 15, 12, 1, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/laptop.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000002"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000004"), new DateTime(2023, 1, 15, 12, 1, 10, 0, DateTimeKind.Utc), 2, "/uploads/products/laptop_klavye.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000002"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000005"), new DateTime(2023, 1, 15, 12, 2, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/tablet.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000003"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000006"), new DateTime(2023, 1, 15, 12, 3, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/kulaklik.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000004"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000007"), new DateTime(2023, 2, 1, 9, 0, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/tshirt.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000005"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000008"), new DateTime(2023, 2, 1, 9, 0, 10, 0, DateTimeKind.Utc), 2, "/uploads/products/tshirt_model.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000005"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000009"), new DateTime(2023, 2, 1, 9, 1, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/jean.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000006"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000010"), new DateTime(2023, 2, 1, 9, 2, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/yagmurluk.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000007"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000011"), new DateTime(2023, 3, 5, 14, 0, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/blender.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000008"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000012"), new DateTime(2023, 3, 5, 14, 1, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/kahve_makinesi.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000009"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000013"), new DateTime(2023, 4, 10, 16, 0, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/bilim_kurgu_kitap.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000010"), null },
                    { new Guid("c1d2e3f4-a5b6-0003-0000-000000000014"), new DateTime(2023, 4, 10, 16, 1, 5, 0, DateTimeKind.Utc), 1, "/uploads/products/tarih_kitap.jpg", false, new Guid("b1c2d3e4-f5a6-0002-0000-000000000011"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
