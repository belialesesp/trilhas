using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class renomeartabela : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_ITipoDeSolucao_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_ITipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "ITipoDeSolucao");

            migrationBuilder.DropForeignKey(
                name: "FK_ITipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "ITipoDeSolucao");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_ITipoDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ITipoDeSolucao",
                table: "ITipoDeSolucao");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Eixos");

            migrationBuilder.RenameTable(
                name: "ITipoDeSolucao",
                newName: "TiposDeSolucao");

            migrationBuilder.RenameIndex(
                name: "IX_ITipoDeSolucao_TipoDoCursoId",
                table: "TiposDeSolucao",
                newName: "IX_TiposDeSolucao_TipoDoCursoId");

            migrationBuilder.RenameIndex(
                name: "IX_ITipoDeSolucao_NivelDoCursoId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "ITipoDeSolucao");

            migrationBuilder.RenameIndex(
                name: "IX_TiposDeSolucao_TipoDoCursoId",
                table: "ITipoDeSolucao",
                newName: "IX_ITipoDeSolucao_TipoDoCursoId");

            migrationBuilder.RenameIndex(
                name: "IX_TiposDeSolucao_NivelDoCursoId",
                table: "ITipoDeSolucao",
                newName: "IX_ITipoDeSolucao_NivelDoCursoId");

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Eixos",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITipoDeSolucao",
                table: "ITipoDeSolucao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_ITipoDeSolucao_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "ITipoDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ITipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                table: "ITipoDeSolucao",
                column: "NivelDoCursoId",
                principalTable: "NiveisDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ITipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                table: "ITipoDeSolucao",
                column: "TipoDoCursoId",
                principalTable: "TiposDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_ITipoDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                column: "TipoDeSolucaoId",
                principalTable: "ITipoDeSolucao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
