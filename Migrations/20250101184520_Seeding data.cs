using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeekStore.API.Migrations
{
    /// <inheritdoc />
    public partial class Seedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tiers",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("05547253-358b-4923-b34f-9abf8b96fb61"), null, "Low end" },
                    { new Guid("bec04c25-6ba3-46f9-9dd5-273a042cba80"), null, "High end" },
                    { new Guid("d43469ab-503e-453e-a35a-075752fe84d6"), null, "Mid end" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DeleteData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("05547253-358b-4923-b34f-9abf8b96fb61"));

            migrationBuilder.DeleteData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("bec04c25-6ba3-46f9-9dd5-273a042cba80"));

            migrationBuilder.DeleteData(
                table: "Tiers",
                keyColumn: "Id",
                keyValue: new Guid("d43469ab-503e-453e-a35a-075752fe84d6"));
        }
    }
}
