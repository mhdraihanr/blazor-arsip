using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blazor_arsip.Migrations
{
    /// <inheritdoc />
    public partial class AddAuth0Integration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth0Id",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 335, DateTimeKind.Utc).AddTicks(9809));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1846));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1851));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1853));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1854));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1856));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1857));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 16, 2, 36, 48, 336, DateTimeKind.Utc).AddTicks(1859));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Auth0Id",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Auth0Id",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Auth0Id",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth0Id",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(2646));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3864));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3867));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3868));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3869));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3869));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3870));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 9, 48, 54, 153, DateTimeKind.Utc).AddTicks(3871));
        }
    }
}
