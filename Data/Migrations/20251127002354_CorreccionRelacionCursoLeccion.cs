using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCursos.Data.Migrations
{
	public partial class CorreccionRelacionCursoLeccion : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Quitar FK vieja si existe
			migrationBuilder.DropForeignKey(
				name: "FK_Leccion_Curso_CursoId",
				table: "Leccion");

			// Eliminar columna duplicada IdCurso (solo UNA VEZ)
			migrationBuilder.DropColumn(
				name: "IdCurso",
				table: "Leccion");

			// Asegurar que CursoId sea INT NOT NULL
			migrationBuilder.AlterColumn<int>(
				name: "CursoId",
				table: "Leccion",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			// FK correcta
			migrationBuilder.AddForeignKey(
				name: "FK_Leccion_Curso_CursoId",
				table: "Leccion",
				column: "CursoId",
				principalTable: "Curso",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Quitar FK correcta
			migrationBuilder.DropForeignKey(
				name: "FK_Leccion_Curso_CursoId",
				table: "Leccion");

			// Volver a nullable
			migrationBuilder.AlterColumn<int>(
				name: "CursoId",
				table: "Leccion",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			// Volver a agregar IdCurso si hiciera falta
			migrationBuilder.AddColumn<int>(
				name: "IdCurso",
				table: "Leccion",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddForeignKey(
				name: "FK_Leccion_Curso_CursoId",
				table: "Leccion",
				column: "CursoId",
				principalTable: "Curso",
				principalColumn: "Id");
		}
	}
}
