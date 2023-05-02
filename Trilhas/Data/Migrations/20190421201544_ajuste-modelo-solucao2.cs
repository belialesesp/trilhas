using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustemodelosolucao2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_TipoDeSolucao_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_TipoDeSolucao_TipoDeSolucaoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropTable(
                name: "TipoDeSolucao");

            migrationBuilder.RenameColumn(
                name: "TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                newName: "TipoDoCursoId");

            migrationBuilder.RenameIndex(
                name: "IX_SolucoesEducacionais_TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                newName: "IX_SolucoesEducacionais_TipoDoCursoId");

            migrationBuilder.AddColumn<string>(
                name: "ConteudoProgramatico",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaMinimaCertificado",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaMinimaDeclaracao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NivelDoCursoId",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteCertificado",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreRequisitos",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicoAlvo",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sigla",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPublicacao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Edicao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Editora",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutrasInformacoes",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Livro_Titulo",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_solucao",
                table: "SolucoesEducacionais",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProducao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duracao",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_OutrasInformacoes",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsavel",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Titulo",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Url",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_NivelDoCursoId",
                table: "SolucoesEducacionais",
                column: "NivelDoCursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_NiveisDeCurso_NivelDoCursoId",
                table: "SolucoesEducacionais",
                column: "NivelDoCursoId",
                principalTable: "NiveisDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeCurso_TipoDoCursoId",
                table: "SolucoesEducacionais",
                column: "TipoDoCursoId",
                principalTable: "TiposDeCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_NiveisDeCurso_NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeCurso_TipoDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "ConteudoProgramatico",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "FrequenciaMinimaCertificado",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "FrequenciaMinimaDeclaracao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "PermiteCertificado",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "PreRequisitos",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "PublicoAlvo",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Sigla",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Autor",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "DataPublicacao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Edicao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Editora",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "OutrasInformacoes",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Livro_Titulo",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "tipo_solucao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "DataProducao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Duracao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_OutrasInformacoes",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Responsavel",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_Titulo",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_Url",
                table: "SolucoesEducacionais");

            migrationBuilder.RenameColumn(
                name: "TipoDoCursoId",
                table: "SolucoesEducacionais",
                newName: "TipoDeSolucaoId");

            migrationBuilder.RenameIndex(
                name: "IX_SolucoesEducacionais_TipoDoCursoId",
                table: "SolucoesEducacionais",
                newName: "IX_SolucoesEducacionais_TipoDeSolucaoId");

            migrationBuilder.CreateTable(
                name: "TipoDeSolucao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    ConteudoProgramatico = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: true),
                    FrequenciaMinimaCertificado = table.Column<int>(nullable: true),
                    FrequenciaMinimaDeclaracao = table.Column<int>(nullable: true),
                    NivelDoCursoId = table.Column<long>(nullable: true),
                    PermiteCertificado = table.Column<bool>(nullable: true),
                    PreRequisitos = table.Column<string>(nullable: true),
                    PublicoAlvo = table.Column<string>(nullable: true),
                    Sigla = table.Column<string>(nullable: true),
                    TipoDoCursoId = table.Column<long>(nullable: true),
                    Titulo = table.Column<string>(nullable: true),
                    Autor = table.Column<string>(nullable: true),
                    DataPublicacao = table.Column<DateTime>(nullable: true),
                    Edicao = table.Column<string>(nullable: true),
                    Editora = table.Column<string>(nullable: true),
                    OutrasInformacoes = table.Column<string>(nullable: true),
                    Livro_Titulo = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    DataProducao = table.Column<DateTime>(nullable: true),
                    Duracao = table.Column<string>(nullable: true),
                    Video_OutrasInformacoes = table.Column<string>(nullable: true),
                    Responsavel = table.Column<string>(nullable: true),
                    Video_Titulo = table.Column<string>(nullable: true),
                    Video_Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDeSolucao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                        column: x => x.NivelDoCursoId,
                        principalTable: "NiveisDeCurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                        column: x => x.TipoDoCursoId,
                        principalTable: "TiposDeCurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TipoDeSolucao_NivelDoCursoId",
                table: "TipoDeSolucao",
                column: "NivelDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoDeSolucao_TipoDoCursoId",
                table: "TipoDeSolucao",
                column: "TipoDoCursoId");

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
        }
    }
}
