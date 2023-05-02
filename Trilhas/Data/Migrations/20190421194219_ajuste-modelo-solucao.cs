using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustemodelosolucao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_TiposDeSolucao_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "TiposDeSolucao");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "TiposDeSolucao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposDeSolucao",
                table: "TiposDeSolucao");

            migrationBuilder.RenameTable(
                name: "TiposDeSolucao",
                newName: "TipoDeSolucao");

            migrationBuilder.RenameIndex(
                name: "IX_TiposDeSolucao_TipoDoCursoId",
                table: "TipoDeSolucao",
                newName: "IX_TipoDeSolucao_TipoDoCursoId");

            migrationBuilder.RenameIndex(
                name: "IX_TiposDeSolucao_NivelDoCursoId",
                table: "TipoDeSolucao",
                newName: "IX_TipoDeSolucao_NivelDoCursoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoDeSolucao",
                table: "TipoDeSolucao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_TipoDeSolucao_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "TipoDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_TipoDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                column: "TipoDeSolucaoId",
                principalTable: "TipoDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "TipoDeSolucao",
                column: "NivelDoCursoId",
                principalTable: "NiveisDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "TipoDeSolucao",
                column: "TipoDoCursoId",
                principalTable: "TiposDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_TipoDeSolucao_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_TipoDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropForeignKey(
                name: "FK_TipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "TipoDeSolucao");

            migrationBuilder.DropForeignKey(
                name: "FK_TipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "TipoDeSolucao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoDeSolucao",
                table: "TipoDeSolucao");

            migrationBuilder.RenameTable(
                name: "TipoDeSolucao",
                newName: "TiposDeSolucao");

            migrationBuilder.RenameIndex(
                name: "IX_TipoDeSolucao_TipoDoCursoId",
                table: "TiposDeSolucao",
                newName: "IX_TiposDeSolucao_TipoDoCursoId");

            migrationBuilder.RenameIndex(
                name: "IX_TipoDeSolucao_NivelDoCursoId",
                table: "TiposDeSolucao",
                newName: "IX_TiposDeSolucao_NivelDoCursoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposDeSolucao",
                table: "TiposDeSolucao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_TiposDeSolucao_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "TiposDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                column: "TipoDeSolucaoId",
                principalTable: "TiposDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "TiposDeSolucao",
                column: "NivelDoCursoId",
                principalTable: "NiveisDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "TiposDeSolucao",
                column: "TipoDoCursoId",
                principalTable: "TiposDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
