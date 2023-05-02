using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaoentidadeDocenteaddcampoPessoa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Docente");

            migrationBuilder.AddColumn<long>(
                name: "PessoaId",
                table: "Docente",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docente_PessoaId",
                table: "Docente",
                column: "PessoaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Docente_Pessoa_PessoaId",
                table: "Docente",
                column: "PessoaId",
                principalTable: "Pessoa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Docente_Pessoa_PessoaId",
                table: "Docente");

            migrationBuilder.DropIndex(
                name: "IX_Docente_PessoaId",
                table: "Docente");

            migrationBuilder.DropColumn(
                name: "PessoaId",
                table: "Docente");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Docente",
                nullable: true);
        }
    }
}
