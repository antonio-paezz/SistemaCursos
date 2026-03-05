using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCursos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoTipoACurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "Curso",
                type: "nvarchar(20)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Curso");
        }
    }
}
