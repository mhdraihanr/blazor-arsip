using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blazor_arsip.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDownloadCountColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "FileRecords");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "FileRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 874, DateTimeKind.Utc).AddTicks(9859));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1059));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1061));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1063));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1064));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1065));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1066));

            migrationBuilder.UpdateData(
                table: "FileCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 2, 9, 37, 14, 875, DateTimeKind.Utc).AddTicks(1067));
        }
    }
}
