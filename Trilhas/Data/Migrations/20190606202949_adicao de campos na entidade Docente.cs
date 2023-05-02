using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class adicaodecamposnaentidadeDocente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Docentes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DadosBancarios",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocenteId = table.Column<long>(nullable: true),
                    Banco = table.Column<string>(nullable: true),
                    ContaCorrente = table.Column<string>(nullable: true),
                    Agencia = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosBancarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DadosBancarios_Docente_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Formacao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocenteId = table.Column<long>(nullable: true),
                    Curso = table.Column<string>(nullable: true),
                    Titulacao = table.Column<string>(nullable: true),
                    Instituicao = table.Column<string>(nullable: true),
                    CargaHoraria = table.Column<int>(nullable: false),
                    DataInicio = table.Column<DateTime>(nullable: false),
                    DataFim = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formacao_Docente_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Habilitacao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocenteId = table.Column<long>(nullable: true),
                    CursoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilitacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habilitacao_SolucoesEducacionais_CursoId",
                        column: x => x.CursoId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Habilitacao_Docente_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DadosBancarios_DocenteId",
                table: "DadosBancarios",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Formacao_DocenteId",
                table: "Formacao",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Habilitacao_CursoId",
                table: "Habilitacao",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Habilitacao_DocenteId",
                table: "Habilitacao",
                column: "DocenteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosBancarios");

            migrationBuilder.DropTable(
                name: "Formacao");

            migrationBuilder.DropTable(
                name: "Habilitacao");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Docentes");
        }
    }
}
