using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class modulos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_SolucoesEducacionais_CursoId",
                table: "Modulo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo");

            migrationBuilder.RenameTable(
                name: "Modulo",
                newName: "Modulos");

            migrationBuilder.RenameIndex(
                name: "IX_Modulo_CursoId",
                table: "Modulos",
                newName: "IX_Modulos_CursoId");

            migrationBuilder.AlterColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Modulos",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modulos",
                table: "Modulos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_SolucaoEducacionalId",
                table: "Modulos",
                column: "SolucaoEducacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_CursoId",
                table: "Modulos",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Modulos",
                column: "SolucaoEducacionalId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_CursoId",
                table: "Modulos");

            migrationBuilder.DropForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Modulos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modulos",
                table: "Modulos");

            migrationBuilder.DropIndex(
                name: "IX_Modulos_SolucaoEducacionalId",
                table: "Modulos");

            migrationBuilder.RenameTable(
                name: "Modulos",
                newName: "Modulo");

            migrationBuilder.RenameIndex(
                name: "IX_Modulos_CursoId",
                table: "Modulo",
                newName: "IX_Modulo_CursoId");

            migrationBuilder.AlterColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Modulo",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modulo",
                table: "Modulo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_SolucoesEducacionais_CursoId",
                table: "Modulo",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
