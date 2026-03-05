using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeCursos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionRelacionesCategoriaInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curso_AspNetUsers_InstructorId",
                table: "Curso");

            migrationBuilder.DropColumn(
                name: "IdInstructor",
                table: "Curso");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Curso",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Curso_AspNetUsers_InstructorId",
                table: "Curso",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);


			

			// Eliminar columna incorrecta
			migrationBuilder.DropColumn(
				name: "IdCategoria",
				table: "Curso");


		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curso_AspNetUsers_InstructorId",
                table: "Curso");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Curso",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");


            migrationBuilder.AddColumn<string>(
                name: "IdInstructor",
                table: "Curso",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Curso_AspNetUsers_InstructorId",
                table: "Curso",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

			
            

			

			migrationBuilder.AddColumn<int>(
				name: "IdCategoria",
				table: "Curso",
				type: "int",
				nullable: false,
				defaultValue: 0);

		}
    }
}
