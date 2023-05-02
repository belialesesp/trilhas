using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class Penalidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inscricao");

            migrationBuilder.DropTable(
                name: "ListaPresenca");

            migrationBuilder.CreateTable(
                name: "Inscritos",
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
                    PessoaId = table.Column<long>(nullable: true),
                    DataDeInscricao = table.Column<DateTime>(nullable: false),
                    PercentualDeFrequencia = table.Column<double>(nullable: false),
                    ListaDeInscricaoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscritos_ListaDeInscricao_ListaDeInscricaoId",
                        column: x => x.ListaDeInscricaoId,
                        principalTable: "ListaDeInscricao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inscritos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Penalidades",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PessoaId = table.Column<long>(nullable: true),
                    DataInicioPenalidade = table.Column<DateTime>(nullable: false),
                    DataFimPenalidade = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalidades_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosDePresenca",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventoHorarioId = table.Column<long>(nullable: true),
                    PessoaId = table.Column<long>(nullable: true),
                    Presente = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosDePresenca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosDePresenca_EventoHorario_EventoHorarioId",
                        column: x => x.EventoHorarioId,
                        principalTable: "EventoHorario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosDePresenca_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscritos_ListaDeInscricaoId",
                table: "Inscritos",
                column: "ListaDeInscricaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscritos_PessoaId",
                table: "Inscritos",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalidades_PessoaId",
                table: "Penalidades",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDePresenca_EventoHorarioId",
                table: "RegistrosDePresenca",
                column: "EventoHorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDePresenca_PessoaId",
                table: "RegistrosDePresenca",
                column: "PessoaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inscritos");

            migrationBuilder.DropTable(
                name: "Penalidades");

            migrationBuilder.DropTable(
                name: "RegistrosDePresenca");

            migrationBuilder.CreateTable(
                name: "Inscricao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<string>(nullable: true),
                    DataDeInscricao = table.Column<DateTime>(nullable: false),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DeletionUserId = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<string>(nullable: true),
                    ListaDeInscricaoId = table.Column<long>(nullable: true),
                    PessoaId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscricao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscricao_ListaDeInscricao_ListaDeInscricaoId",
                        column: x => x.ListaDeInscricaoId,
                        principalTable: "ListaDeInscricao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inscricao_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListaPresenca",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventoHorarioId = table.Column<long>(nullable: true),
                    PessoaId = table.Column<long>(nullable: true),
                    Presente = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaPresenca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaPresenca_EventoHorario_EventoHorarioId",
                        column: x => x.EventoHorarioId,
                        principalTable: "EventoHorario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListaPresenca_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscricao_ListaDeInscricaoId",
                table: "Inscricao",
                column: "ListaDeInscricaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricao_PessoaId",
                table: "Inscricao",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresenca_EventoHorarioId",
                table: "ListaPresenca",
                column: "EventoHorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresenca_PessoaId",
                table: "ListaPresenca",
                column: "PessoaId");
        }
    }
}
