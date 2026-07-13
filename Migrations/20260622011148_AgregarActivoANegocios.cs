using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KovakWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarActivoANegocios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Negocios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Negocios");
        }
    }
}
