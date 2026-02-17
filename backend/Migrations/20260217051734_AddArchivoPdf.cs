using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivoPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Archivo_Nutricionista",
                columns: table => new
                {
                    Id_Archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Nutricionista = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RutaAcceso = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaSubida = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archivo_Nutricionista", x => x.Id_Archivo);
                    table.ForeignKey(
                        name: "FK_Archivo_Nutricionista_Nutricionista_Id_Nutricionista",
                        column: x => x.Id_Nutricionista,
                        principalTable: "Nutricionista",
                        principalColumn: "Id_Nutricionista",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Archivo_Nutricionista_Id_Nutricionista",
                table: "Archivo_Nutricionista",
                column: "Id_Nutricionista");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archivo_Nutricionista");
        }
    }
}
