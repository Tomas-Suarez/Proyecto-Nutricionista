using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AccesoPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_Usuario_Id_Usuario",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_Id_Usuario",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Id_Usuario",
                table: "Paciente");

            migrationBuilder.AddColumn<string>(
                name: "CodigoAcceso",
                table: "Paciente",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Paciente",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TokenAcceso",
                table: "Paciente",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoAcceso",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "TokenAcceso",
                table: "Paciente");

            migrationBuilder.AddColumn<int>(
                name: "Id_Usuario",
                table: "Paciente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_Id_Usuario",
                table: "Paciente",
                column: "Id_Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_Usuario_Id_Usuario",
                table: "Paciente",
                column: "Id_Usuario",
                principalTable: "Usuario",
                principalColumn: "Id_Usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
