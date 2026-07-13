using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KovakWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCarruselFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlImagen2",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen3",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen4",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImagen2",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UrlImagen3",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UrlImagen4",
                table: "Productos");
        }
    }
}
