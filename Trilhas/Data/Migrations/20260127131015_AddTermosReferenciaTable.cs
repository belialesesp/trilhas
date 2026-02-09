using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trilhas.Data.Migrations
{
    public partial class AddTermosReferenciaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Curso_CursoId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_Curso_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Habilitacao_Curso_CursoId",
                table: "Habilitacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Modulos_Curso_CursoId",
                table: "Modulos");

            migrationBuilder.DropTable(
                name: "Curso");

            migrationBuilder.DropTable(
                name: "Livro");

            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConteudoProgramatico",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProducao",
                table: "SolucoesEducacionais",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPublicacao",
                table: "SolucoesEducacionais",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duracao",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Edicao",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Editora",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaMinimaCertificado",
                table: "SolucoesEducacionais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaMinimaDeclaracao",
                table: "SolucoesEducacionais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Modalidade",
                table: "SolucoesEducacionais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NivelDoCursoId",
                table: "SolucoesEducacionais",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutrasInformacoes",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteCertificado",
                table: "SolucoesEducacionais",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreRequisitos",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicoAlvo",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsavel",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sigla",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TipoDoCursoId",
                table: "SolucoesEducacionais",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_OutrasInformacoes",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Url",
                table: "SolucoesEducacionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CertificadosEmitidos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PessoaId = table.Column<long>(type: "bigint", nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoAutenticacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletionUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificadosEmitidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificadosEmitidos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GridCursistaDto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantidadeEvento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridCursistaDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GridDocenteDto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Excluido = table.Column<int>(type: "int", nullable: false),
                    QuantidadeEvento = table.Column<int>(type: "int", nullable: false),
                    CargaHorariaTotal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridDocenteDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermosDeReferencia",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CaminhoArquivoOriginal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifierUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletionUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosDeReferencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermoReferenciaItens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermoDeReferenciaId = table.Column<long>(type: "bigint", nullable: false),
                    Curso = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Profissional = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    CargaHoraria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MesExecucao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataOferta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modalidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuantidadeTurmas = table.Column<int>(type: "int", nullable: false),
                    AlunosPorTurma = table.Column<int>(type: "int", nullable: false),
                    ValorHora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EncargosPercentual = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifierUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletionUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermoReferenciaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermoReferenciaItens_TermosDeReferencia_TermoDeReferenciaId",
                        column: x => x.TermoDeReferenciaId,
                        principalTable: "TermosDeReferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Nome",
                value: "ALFABETIZADO");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Nome",
                value: "ANALFABETO");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Nome",
                value: "DOUTOR");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Nome",
                value: "MESTRE");

            migrationBuilder.InsertData(
                table: "Escolaridades",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 5L, "PÓS GRADUADO" },
                    { 6L, "PRIMEIRO GRAU COMPLETO" },
                    { 7L, "PRIMEIRO GRAU INCOMPLETO" },
                    { 8L, "SEGUNDO GRAU COMPLETO" },
                    { 9L, "SEGUNDO GRAU INCOMPLETO" },
                    { 10L, "SUPERIOR COMPLETO" },
                    { 11L, "SUPERIOR INCOMPLETO" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_NivelDoCursoId",
                table: "SolucoesEducacionais",
                column: "NivelDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_TipoDoCursoId",
                table: "SolucoesEducacionais",
                column: "TipoDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_PessoaId",
                table: "CertificadosEmitidos",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_TermoReferenciaItens_TermoDeReferenciaId",
                table: "TermoReferenciaItens",
                column: "TermoDeReferenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_SolucoesEducacionais_CursoId",
                table: "Eventos",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilitacao_SolucoesEducacionais_CursoId",
                table: "Habilitacao",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_CursoId",
                table: "Modulos",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_NiveisDeCurso_NivelDoCursoId",
                table: "SolucoesEducacionais",
                column: "NivelDoCursoId",
                principalTable: "NiveisDeCurso",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeCurso_TipoDoCursoId",
                table: "SolucoesEducacionais",
                column: "TipoDoCursoId",
                principalTable: "TiposDeCurso",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_SolucoesEducacionais_CursoId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_CursoId",
                table: "Habilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Habilitacao_SolucoesEducacionais_CursoId",
                table: "Habilitacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_CursoId",
                table: "Modulos");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_NiveisDeCurso_NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_TiposDeCurso_TipoDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropTable(
                name: "CertificadosEmitidos");

            migrationBuilder.DropTable(
                name: "GridCursistaDto");

            migrationBuilder.DropTable(
                name: "GridDocenteDto");

            migrationBuilder.DropTable(
                name: "TermoReferenciaItens");

            migrationBuilder.DropTable(
                name: "TermosDeReferencia");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_TipoDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DropColumn(
                name: "Autor",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "ConteudoProgramatico",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "DataProducao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "DataPublicacao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Duracao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Edicao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Editora",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "FrequenciaMinimaCertificado",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "FrequenciaMinimaDeclaracao",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Modalidade",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "NivelDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "OutrasInformacoes",
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
                name: "Responsavel",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Sigla",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "TipoDoCursoId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_OutrasInformacoes",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_Url",
                table: "SolucoesEducacionais");

            migrationBuilder.CreateTable(
                name: "Curso",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    NivelDoCursoId = table.Column<long>(type: "bigint", nullable: true),
                    TipoDoCursoId = table.Column<long>(type: "bigint", nullable: true),
                    ConteudoProgramatico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrequenciaMinimaCertificado = table.Column<int>(type: "int", nullable: true),
                    FrequenciaMinimaDeclaracao = table.Column<int>(type: "int", nullable: true),
                    Modalidade = table.Column<int>(type: "int", nullable: true),
                    PermiteCertificado = table.Column<bool>(type: "bit", nullable: true),
                    PreRequisitos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicoAlvo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Curso_NiveisDeCurso_NivelDoCursoId",
                        column: x => x.NivelDoCursoId,
                        principalTable: "NiveisDeCurso",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Curso_TiposDeCurso_TipoDoCursoId",
                        column: x => x.TipoDoCursoId,
                        principalTable: "TiposDeCurso",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Livro",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPublicacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edicao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Editora = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutrasInformacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DataProducao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duracao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Video_OutrasInformacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Video_Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Nome",
                value: "1º GRAU INCOMPLETO");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Nome",
                value: "1º GRAU COMPLETO");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Nome",
                value: "ENSINO SUPERIOR INCOMPLETO");

            migrationBuilder.UpdateData(
                table: "Escolaridades",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Nome",
                value: "ENSINO SUPERIOR COMPLETO");

            migrationBuilder.CreateIndex(
                name: "IX_Curso_NivelDoCursoId",
                table: "Curso",
                column: "NivelDoCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Curso_TipoDoCursoId",
                table: "Curso",
                column: "TipoDoCursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Curso_CursoId",
                table: "Eventos",
                column: "CursoId",
                principalTable: "Curso",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_Curso_CursoId",
                table: "Habilidades",
                column: "CursoId",
                principalTable: "Curso",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilitacao_Curso_CursoId",
                table: "Habilitacao",
                column: "CursoId",
                principalTable: "Curso",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulos_Curso_CursoId",
                table: "Modulos",
                column: "CursoId",
                principalTable: "Curso",
                principalColumn: "Id");
        }
    }
}
