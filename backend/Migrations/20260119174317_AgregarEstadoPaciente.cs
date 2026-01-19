using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Peso_Actual",
                table: "Paciente");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Paciente",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Paciente");

            migrationBuilder.AddColumn<decimal>(
                name: "Peso_Actual",
                table: "Paciente",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
