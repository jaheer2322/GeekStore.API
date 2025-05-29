using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekStore.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_CategoryId_TierId",
                table: "Products",
                columns: new[] { "Name", "CategoryId", "TierId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name_CategoryId_TierId",
                table: "Products");
        }
    }
}
