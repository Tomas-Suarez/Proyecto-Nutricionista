using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class CambiosPesajee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nota",
                table: "Pesaje",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Masa_Muscular_Kg",
                table: "Pesaje",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Porcentaje_Grasa",
                table: "Pesaje",
                type: "decimal(4,1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Masa_Muscular_Kg",
                table: "Pesaje");

            migrationBuilder.DropColumn(
                name: "Porcentaje_Grasa",
                table: "Pesaje");

            migrationBuilder.UpdateData(
                table: "Pesaje",
                keyColumn: "Nota",
                keyValue: null,
                column: "Nota",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Nota",
                table: "Pesaje",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
