using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blazor_arsip.Migrations
{
    /// <inheritdoc />
    public partial class FixLoginIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(2092));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3364));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3366));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3367));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3368));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3369));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3370));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 7, 9, 29, 56, 534, DateTimeKind.Utc).AddTicks(3371));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
