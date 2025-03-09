using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeekStore.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingCategorydata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tiers");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1992b5e0-7888-476b-a46d-ce812e8d7b6d"), "GPU" },
                    { new Guid("5ec4a3f7-b00a-47a3-aa3d-d946030ca55c"), "Ram" },
                    { new Guid("6a3fb4b3-2c2b-4f0e-8cbb-9b4d914729b1"), "CPU" },
                    { new Guid("8499e196-2cb1-45ad-b7bd-a82a0bb48745"), "Motherboard" },
                    { new Guid("9e336f6c-e645-49a7-bd6f-38f79cdf548a"), "PSU" },
                    { new Guid("a24ad4ff-ad4a-4dd7-8ac0-53a6216ab93f"), "Miscellaneous" },
                    { new Guid("be730ab1-9f45-41ab-a094-bc1b8a301a03"), "Graphics card" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1992b5e0-7888-476b-a46d-ce812e8d7b6d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("5ec4a3f7-b00a-47a3-aa3d-d946030ca55c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("6a3fb4b3-2c2b-4f0e-8cbb-9b4d914729b1"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8499e196-2cb1-45ad-b7bd-a82a0bb48745"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9e336f6c-e645-49a7-bd6f-38f79cdf548a"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a24ad4ff-ad4a-4dd7-8ac0-53a6216ab93f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("be730ab1-9f45-41ab-a094-bc1b8a301a03"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tiers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("05547253-358b-4923-b34f-9abf8b96fb61"),
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("bec04c25-6ba3-46f9-9dd5-273a042cba80"),
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("d43469ab-503e-453e-a35a-075752fe84d6"),
                column: "Description",
                value: null);
        }
    }
}
