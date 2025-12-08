using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ModeloComple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaComida_Categoria_Id_Categoria",
                table: "CategoriaComida");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaComida_Comida_Id_Comida",
                table: "CategoriaComida");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriaComida",
                table: "CategoriaComida");

            migrationBuilder.RenameTable(
                name: "CategoriaComida",
                newName: "Categoria_Comida");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComida_Id_Comida",
                table: "Categoria_Comida",
                newName: "IX_Categoria_Comida_Id_Comida");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComida_Id_Categoria",
                table: "Categoria_Comida",
                newName: "IX_Categoria_Comida_Id_Categoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categoria_Comida",
                table: "Categoria_Comida",
                column: "Id_Categoria_Comida");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_Comida_Categoria_Id_Categoria",
                table: "Categoria_Comida",
                column: "Id_Categoria",
                principalTable: "Categoria",
                principalColumn: "Id_Categoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_Comida_Comida_Id_Comida",
                table: "Categoria_Comida",
                column: "Id_Comida",
                principalTable: "Comida",
                principalColumn: "Id_Comida",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_Comida_Categoria_Id_Categoria",
                table: "Categoria_Comida");

            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_Comida_Comida_Id_Comida",
                table: "Categoria_Comida");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categoria_Comida",
                table: "Categoria_Comida");

            migrationBuilder.RenameTable(
                name: "Categoria_Comida",
                newName: "CategoriaComida");

            migrationBuilder.RenameIndex(
                name: "IX_Categoria_Comida_Id_Comida",
                table: "CategoriaComida",
                newName: "IX_CategoriaComida_Id_Comida");

            migrationBuilder.RenameIndex(
                name: "IX_Categoria_Comida_Id_Categoria",
                table: "CategoriaComida",
                newName: "IX_CategoriaComida_Id_Categoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriaComida",
                table: "CategoriaComida",
                column: "Id_Categoria_Comida");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaComida_Categoria_Id_Categoria",
                table: "CategoriaComida",
                column: "Id_Categoria",
                principalTable: "Categoria",
                principalColumn: "Id_Categoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaComida_Comida_Id_Comida",
                table: "CategoriaComida",
                column: "Id_Comida",
                principalTable: "Comida",
                principalColumn: "Id_Comida",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
