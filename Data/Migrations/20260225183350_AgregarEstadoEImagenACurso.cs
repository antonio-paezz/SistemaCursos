using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCursos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoEImagenACurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Curso",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Curso",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Curso",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Curso");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Curso");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Curso");
        }
    }
}
