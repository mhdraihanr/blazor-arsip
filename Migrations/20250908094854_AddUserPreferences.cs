using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blazor_arsip.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DarkMode = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "en")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemsPerPage = table.Column<int>(type: "int", nullable: false, defaultValue: 25),
                    AutoSave = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    EmailNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    BrowserNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DefaultUploadCategory = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaxFileSize = table.Column<int>(type: "int", nullable: false, defaultValue: 50),
                    AutoCategorize = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DeleteConfirmation = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    SessionTimeout = table.Column<int>(type: "int", nullable: false, defaultValue: 120),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferences");

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
    }
}
