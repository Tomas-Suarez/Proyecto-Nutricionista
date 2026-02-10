using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class NuevoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Altura_Cm",
                table: "Paciente",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<int>(
                name: "Id_Objetivo",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Objetivo",
                columns: table => new
                {
                    Id_Objetivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivo", x => x.Id_Objetivo);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Patologia",
                columns: table => new
                {
                    Id_Patologia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patologia", x => x.Id_Patologia);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Patologia_Paciente",
                columns: table => new
                {
                    Id_Patologia = table.Column<int>(type: "int", nullable: false),
                    Id_Paciente = table.Column<int>(type: "int", nullable: false),
                    Id_Patologia_Paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatologiaEntityId_Patologia = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patologia_Paciente", x => new { x.Id_Paciente, x.Id_Patologia });
                    table.ForeignKey(
                        name: "FK_Patologia_Paciente_Paciente_Id_Paciente",
                        column: x => x.Id_Paciente,
                        principalTable: "Paciente",
                        principalColumn: "Id_Paciente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patologia_Paciente_Patologia_Id_Patologia",
                        column: x => x.Id_Patologia,
                        principalTable: "Patologia",
                        principalColumn: "Id_Patologia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patologia_Paciente_Patologia_PatologiaEntityId_Patologia",
                        column: x => x.PatologiaEntityId_Patologia,
                        principalTable: "Patologia",
                        principalColumn: "Id_Patologia");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_Id_Objetivo",
                table: "Paciente",
                column: "Id_Objetivo");

            migrationBuilder.CreateIndex(
                name: "IX_Patologia_Paciente_Id_Patologia",
                table: "Patologia_Paciente",
                column: "Id_Patologia");

            migrationBuilder.CreateIndex(
                name: "IX_Patologia_Paciente_PatologiaEntityId_Patologia",
                table: "Patologia_Paciente",
                column: "PatologiaEntityId_Patologia");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_Objetivo_Id_Objetivo",
                table: "Paciente",
                column: "Id_Objetivo",
                principalTable: "Objetivo",
                principalColumn: "Id_Objetivo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_Objetivo_Id_Objetivo",
                table: "Paciente");

            migrationBuilder.DropTable(
                name: "Objetivo");

            migrationBuilder.DropTable(
                name: "Patologia_Paciente");

            migrationBuilder.DropTable(
                name: "Patologia");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_Id_Objetivo",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Id_Objetivo",
                table: "Paciente");

            migrationBuilder.AlterColumn<decimal>(
                name: "Altura_Cm",
                table: "Paciente",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }
    }
}
