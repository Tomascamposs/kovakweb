using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KovakWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NegocioID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoTalles_ProductoID",
                table: "ProductoTalles",
                column: "ProductoID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoTalles_Productos_ProductoID",
                table: "ProductoTalles",
                column: "ProductoID",
                principalTable: "Productos",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoTalles_Productos_ProductoID",
                table: "ProductoTalles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_ProductoTalles_ProductoID",
                table: "ProductoTalles");
        }
    }
}
