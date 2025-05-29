using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeekStore.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    TierId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Products_Tiers_TierId",
                        column: x => x.TierId,
                        principalTable: "Tiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Tiers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("05547253-358b-4923-b34f-9abf8b96fb61"), "Low end" },
                    { new Guid("bec04c25-6ba3-46f9-9dd5-273a042cba80"), "High end" },
                    { new Guid("d43469ab-503e-453e-a35a-075752fe84d6"), "Mid end" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TierId",
                table: "Products",
                column: "TierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tiers");
        }
    }
}
