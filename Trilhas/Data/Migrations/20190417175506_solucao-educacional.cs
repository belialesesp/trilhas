using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class solucaoeducacional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NiveisDeCurso",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisDeCurso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDeCurso",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDeCurso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITipoDeSolucao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    Titulo = table.Column<string>(nullable: true),
                    Sigla = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: true),
                    TipoDoCursoId = table.Column<long>(nullable: true),
                    NivelDoCursoId = table.Column<long>(nullable: true),
                    PermiteCertificado = table.Column<bool>(nullable: true),
                    FrequenciaMinimaCertificado = table.Column<int>(nullable: true),
                    FrequenciaMinimaDeclaracao = table.Column<int>(nullable: true),
                    PreRequisitos = table.Column<string>(nullable: true),
                    PublicoAlvo = table.Column<string>(nullable: true),
                    ConteudoProgramatico = table.Column<string>(nullable: true),
                    Livro_Titulo = table.Column<string>(nullable: true),
                    Autor = table.Column<string>(nullable: true),
                    DataPublicacao = table.Column<DateTime>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Editora = table.Column<string>(nullable: true),
                    Edicao = table.Column<string>(nullable: true),
                    OutrasInformacoes = table.Column<string>(nullable: true),
                    Video_Titulo = table.Column<string>(nullable: true),
                    Responsavel = table.Column<string>(nullable: true),
                    DataProducao = table.Column<DateTime>(nullable: true),
                    Video_Url = table.Column<string>(nullable: true),
                    Duracao = table.Column<string>(nullable: true),
                    Video_OutrasInformacoes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITipoDeSolucao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITipoDeSolucao_NiveisDeCurso_NivelDoCursoId",
                        column: x => x.NivelDoCursoId,
                        principalTable: "NiveisDeCurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ITipoDeSolucao_TiposDeCurso_TipoDoCursoId",
                        column: x => x.TipoDoCursoId,
                        principalTable: "TiposDeCurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolucaoEducacionalId = table.Column<long>(nullable: false),
                    Descricao = table.Column<string>(nullable: true),
                    CursoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habilidades_ITipoDeSolucao_CursoId",
                        column: x => x.CursoId,
                        principalTable: "ITipoDeSolucao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolucoesEducacionais",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<string>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DeletionUserId = table.Column<string>(nullable: true),
                    EstacaoId = table.Column<long>(nullable: true),
                    TipoDeSolucaoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolucoesEducacionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolucoesEducacionais_Estacoes_EstacaoId",
                        column: x => x.EstacaoId,
                        principalTable: "Estacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolucoesEducacionais_ITipoDeSolucao_TipoDeSolucaoId",
                        column: x => x.TipoDeSolucaoId,
                        principalTable: "ITipoDeSolucao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habilidades_CursoId",
                table: "Habilidades",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_ITipoDeSolucao_NivelDoCursoId",
                table: "ITipoDeSolucao",
                column: "NivelDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_ITipoDeSolucao_TipoDoCursoId",
                table: "ITipoDeSolucao",
                column: "TipoDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_EstacaoId",
                table: "SolucoesEducacionais",
                column: "EstacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_TipoDeSolucaoId",
                table: "SolucoesEducacionais",
                column: "TipoDeSolucaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habilidades");

            migrationBuilder.DropTable(
                name: "SolucoesEducacionais");

            migrationBuilder.DropTable(
                name: "ITipoDeSolucao");

            migrationBuilder.DropTable(
                name: "NiveisDeCurso");

            migrationBuilder.DropTable(
                name: "TiposDeCurso");
        }
    }
}
