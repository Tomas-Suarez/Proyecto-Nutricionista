using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ModeloComplet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaComidas_Categorias_Id_Categoria",
                table: "CategoriaComidas");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaComidas_Comida_Id_Comida",
                table: "CategoriaComidas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriaComidas",
                table: "CategoriaComidas");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "Categoria");

            migrationBuilder.RenameTable(
                name: "CategoriaComidas",
                newName: "CategoriaComida");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComidas_Id_Comida",
                table: "CategoriaComida",
                newName: "IX_CategoriaComida_Id_Comida");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComidas_Id_Categoria",
                table: "CategoriaComida",
                newName: "IX_CategoriaComida_Id_Categoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria",
                column: "Id_Categoria");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria");

            migrationBuilder.RenameTable(
                name: "CategoriaComida",
                newName: "CategoriaComidas");

            migrationBuilder.RenameTable(
                name: "Categoria",
                newName: "Categorias");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComida_Id_Comida",
                table: "CategoriaComidas",
                newName: "IX_CategoriaComidas_Id_Comida");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaComida_Id_Categoria",
                table: "CategoriaComidas",
                newName: "IX_CategoriaComidas_Id_Categoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriaComidas",
                table: "CategoriaComidas",
                column: "Id_Categoria_Comida");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id_Categoria");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaComidas_Categorias_Id_Categoria",
                table: "CategoriaComidas",
                column: "Id_Categoria",
                principalTable: "Categorias",
                principalColumn: "Id_Categoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaComidas_Comida_Id_Comida",
                table: "CategoriaComidas",
                column: "Id_Comida",
                principalTable: "Comida",
                principalColumn: "Id_Comida",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
