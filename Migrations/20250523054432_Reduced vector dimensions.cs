using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace GeekStore.API.Migrations
{
    /// <inheritdoc />
    public partial class Reducedvectordimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Products",
                type: "vector(380)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(768)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Products",
                type: "vector(768)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(380)",
                oldNullable: true);
        }
    }
}
