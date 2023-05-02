using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajusteslocais : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalContatos_TipoLocalContato_TipoContatoId",
                table: "LocalContatos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalRecursos_Recursos_RecursoId",
                table: "LocalRecursos");

            migrationBuilder.AlterColumn<long>(
                name: "RecursoId",
                table: "LocalRecursos",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "TipoContatoId",
                table: "LocalContatos",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_LocalContatos_TipoLocalContato_TipoContatoId",
                table: "LocalContatos",
                column: "TipoContatoId",
                principalTable: "TipoLocalContato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalRecursos_Recursos_RecursoId",
                table: "LocalRecursos",
                column: "RecursoId",
                principalTable: "Recursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalContatos_TipoLocalContato_TipoContatoId",
                table: "LocalContatos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalRecursos_Recursos_RecursoId",
                table: "LocalRecursos");

            migrationBuilder.AlterColumn<long>(
                name: "RecursoId",
                table: "LocalRecursos",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TipoContatoId",
                table: "LocalContatos",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalContatos_TipoLocalContato_TipoContatoId",
                table: "LocalContatos",
                column: "TipoContatoId",
                principalTable: "TipoLocalContato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalRecursos_Recursos_RecursoId",
                table: "LocalRecursos",
                column: "RecursoId",
                principalTable: "Recursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
