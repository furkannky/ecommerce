using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToAllBaseEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProductImages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Categories",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[] { new Guid("a1b2d3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 1, 1, 10, 1, 0, 0, DateTimeKind.Utc), "Her mevsim için kıyafetler", false, "Giyim", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000005"),
                column: "CategoryId",
                value: new Guid("a1b2d3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000006"),
                column: "CategoryId",
                value: new Guid("a1b2d3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000007"),
                column: "CategoryId",
                value: new Guid("a1b2d3d4-e5f6-0001-0000-000000000002"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2d3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"), new DateTime(2023, 1, 1, 10, 1, 0, 0, DateTimeKind.Utc), "Her mevsim için kıyafetler", false, "Giyim", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000005"),
                column: "CategoryId",
                value: new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000006"),
                column: "CategoryId",
                value: new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-0002-0000-000000000007"),
                column: "CategoryId",
                value: new Guid("a1b2c3d4-e5f6-0001-0000-000000000002"));
        }
    }
}
