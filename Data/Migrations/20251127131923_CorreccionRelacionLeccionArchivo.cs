using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCursos.Data.Migrations
{
	public partial class CorreccionRelacionLeccionArchivo : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Quitar FK vieja si existe
			migrationBuilder.DropForeignKey(
				name: "FK_Archivo_Leccion_LeccionId",
				table: "Archivo");

			// Eliminar columna duplicada IdLeccion
			migrationBuilder.DropColumn(
				name: "IdLeccion",
				table: "Archivo");

			// Asegurar que LeccionId sea INT NOT NULL
			migrationBuilder.AlterColumn<int>(
				name: "LeccionId",
				table: "Archivo",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			// FK correcta
			migrationBuilder.AddForeignKey(
				name: "FK_Archivo_Leccion_LeccionId",
				table: "Archivo",
				column: "LeccionId",
				principalTable: "Leccion",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Quitar FK correcta
			migrationBuilder.DropForeignKey(
				name: "FK_Archivo_Leccion_LeccionId",
				table: "Archivo"); 

			// Volver a nullable
			migrationBuilder.AlterColumn<int>(
				name: "LeccionId",
				table: "Archivo",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			// Volver a agregar IdLeccion
			migrationBuilder.AddColumn<int>(
				name: "IdLeccion",
				table: "Archivo",
				type: "int",
				nullable: false,
				defaultValue: 0);

			// Restaurar la FK vieja
			migrationBuilder.AddForeignKey(
				name: "FK_Archivo_Leccion_LeccionId",
				table: "Archivo",
				column: "LeccionId",
				principalTable: "Leccion",
				principalColumn: "Id");
		}
	}
}
