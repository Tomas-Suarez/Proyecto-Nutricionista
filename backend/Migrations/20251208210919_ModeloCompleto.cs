using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ModeloCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id_Categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id_Categoria);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comida",
                columns: table => new
                {
                    Id_Comida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Calorias = table.Column<int>(type: "int", nullable: false),
                    Proteinas = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Carbohidratos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Grasas = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Fibra = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Azucares = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Porcion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Imagen_Url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comida", x => x.Id_Comida);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dieta",
                columns: table => new
                {
                    Id_Dieta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Paciente = table.Column<int>(type: "int", nullable: false),
                    Id_Nutricionista = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha_Inicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Fin = table.Column<DateTime>(type: "datetime(6)", maxLength: 20, nullable: true),
                    Activa = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dieta", x => x.Id_Dieta);
                    table.ForeignKey(
                        name: "FK_Dieta_Nutricionista_Id_Nutricionista",
                        column: x => x.Id_Nutricionista,
                        principalTable: "Nutricionista",
                        principalColumn: "Id_Nutricionista",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dieta_Paciente_Id_Paciente",
                        column: x => x.Id_Paciente,
                        principalTable: "Paciente",
                        principalColumn: "Id_Paciente",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pesaje",
                columns: table => new
                {
                    Id_Pesaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Paciente = table.Column<int>(type: "int", nullable: false),
                    Peso_Kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Fecha_Pesaje = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Nota = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pesaje", x => x.Id_Pesaje);
                    table.ForeignKey(
                        name: "FK_Pesaje_Paciente_Id_Paciente",
                        column: x => x.Id_Paciente,
                        principalTable: "Paciente",
                        principalColumn: "Id_Paciente",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoriaComidas",
                columns: table => new
                {
                    Id_Categoria_Comida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Comida = table.Column<int>(type: "int", nullable: false),
                    Id_Categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaComidas", x => x.Id_Categoria_Comida);
                    table.ForeignKey(
                        name: "FK_CategoriaComidas_Categorias_Id_Categoria",
                        column: x => x.Id_Categoria,
                        principalTable: "Categorias",
                        principalColumn: "Id_Categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriaComidas_Comida_Id_Comida",
                        column: x => x.Id_Comida,
                        principalTable: "Comida",
                        principalColumn: "Id_Comida",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dieta_Comida",
                columns: table => new
                {
                    Id_Dieta_Comida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Dieta = table.Column<int>(type: "int", nullable: false),
                    Id_Comida = table.Column<int>(type: "int", nullable: false),
                    Horario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nota = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dieta_Comida", x => x.Id_Dieta_Comida);
                    table.ForeignKey(
                        name: "FK_Dieta_Comida_Comida_Id_Comida",
                        column: x => x.Id_Comida,
                        principalTable: "Comida",
                        principalColumn: "Id_Comida",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dieta_Comida_Dieta_Id_Dieta",
                        column: x => x.Id_Dieta,
                        principalTable: "Dieta",
                        principalColumn: "Id_Dieta",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaComidas_Id_Categoria",
                table: "CategoriaComidas",
                column: "Id_Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaComidas_Id_Comida",
                table: "CategoriaComidas",
                column: "Id_Comida");

            migrationBuilder.CreateIndex(
                name: "IX_Dieta_Id_Nutricionista",
                table: "Dieta",
                column: "Id_Nutricionista");

            migrationBuilder.CreateIndex(
                name: "IX_Dieta_Id_Paciente",
                table: "Dieta",
                column: "Id_Paciente");

            migrationBuilder.CreateIndex(
                name: "IX_Dieta_Comida_Id_Comida",
                table: "Dieta_Comida",
                column: "Id_Comida");

            migrationBuilder.CreateIndex(
                name: "IX_Dieta_Comida_Id_Dieta",
                table: "Dieta_Comida",
                column: "Id_Dieta");

            migrationBuilder.CreateIndex(
                name: "IX_Pesaje_Id_Paciente",
                table: "Pesaje",
                column: "Id_Paciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaComidas");

            migrationBuilder.DropTable(
                name: "Dieta_Comida");

            migrationBuilder.DropTable(
                name: "Pesaje");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Comida");

            migrationBuilder.DropTable(
                name: "Dieta");
        }
    }
}
