using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace blazor_arsip.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhotoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(5938));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7334));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7336));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7337));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7338));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7340));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 7, 35, 33, 355, DateTimeKind.Utc).AddTicks(7341));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "LastLoginAt", "Name", "PasswordHash", "PhotoUrl" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@company.com", true, null, "Administrator", "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "user@company.com", true, null, "Regular User", "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "test@company.com", true, null, "Test User", "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(5015));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6037));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6039));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6040));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6041));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6042));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6042));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 4, 3, 33, 10, 477, DateTimeKind.Utc).AddTicks(6043));
        }
    }
}
