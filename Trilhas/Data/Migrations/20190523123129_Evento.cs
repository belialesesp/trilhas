using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class Evento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Docente",
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
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Evento",
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
                    GEDTHPessoaId = table.Column<long>(nullable: true),
                    SolucaoEducacionalId = table.Column<long>(nullable: true),
                    EntidadeDemandanteId = table.Column<long>(nullable: true),
                    LocalId = table.Column<long>(nullable: true),
                    Observacao = table.Column<string>(nullable: true),
                    FlagEad = table.Column<bool>(nullable: false),
                    UrlEad = table.Column<string>(nullable: true),
                    Situacao = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evento_Pessoa_GEDTHPessoaId",
                        column: x => x.GEDTHPessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_Entidades_EntidadeDemandanteId",
                        column: x => x.EntidadeDemandanteId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_Local_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Local",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_SolucoesEducacionais_SolucaoEducacionalId",
                        column: x => x.SolucaoEducacionalId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funcao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventoAgenda",
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
                    DataHoraInicio = table.Column<DateTime>(nullable: false),
                    DataHoraFim = table.Column<DateTime>(nullable: false),
                    DataHoraInscricaoInicio = table.Column<DateTime>(nullable: false),
                    DataHoraInscricaoFim = table.Column<DateTime>(nullable: false),
                    NumeroVagas = table.Column<int>(nullable: false),
                    Justificativa = table.Column<string>(nullable: true),
                    EventoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoAgenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoAgenda_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoCota",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntidadeId = table.Column<long>(nullable: true),
                    Quantidade = table.Column<int>(nullable: false),
                    EventoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoCota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoCota_Entidades_EntidadeId",
                        column: x => x.EntidadeId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoCota_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoRecurso",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RecursoId = table.Column<long>(nullable: true),
                    Quantidade = table.Column<int>(nullable: false),
                    EventoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoRecurso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoRecurso_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoRecurso_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoHorario",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolucaoEducacionalId = table.Column<long>(nullable: true),
                    ModuloId = table.Column<long>(nullable: true),
                    DocenteId = table.Column<long>(nullable: true),
                    FuncaoId = table.Column<long>(nullable: true),
                    LocalId = table.Column<long>(nullable: true),
                    LocalSalaId = table.Column<long>(nullable: true),
                    DataHoraInicio = table.Column<DateTime>(nullable: false),
                    DataHoraFim = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoHorario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoHorario_Docente_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoHorario_Funcao_FuncaoId",
                        column: x => x.FuncaoId,
                        principalTable: "Funcao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoHorario_Local_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Local",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoHorario_LocalSalas_LocalSalaId",
                        column: x => x.LocalSalaId,
                        principalTable: "LocalSalas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoHorario_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoHorario_SolucoesEducacionais_SolucaoEducacionalId",
                        column: x => x.SolucaoEducacionalId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoInstrutor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InstrutorPessoaId = table.Column<long>(nullable: true),
                    SolucaoEducacionalId = table.Column<long>(nullable: true),
                    ModuloId = table.Column<long>(nullable: true),
                    FuncaoId = table.Column<long>(nullable: true),
                    DataHoraInicio = table.Column<DateTime>(nullable: false),
                    DataHoraFim = table.Column<DateTime>(nullable: false),
                    EventoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoInstrutor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoInstrutor_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoInstrutor_Funcao_FuncaoId",
                        column: x => x.FuncaoId,
                        principalTable: "Funcao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoInstrutor_Pessoa_InstrutorPessoaId",
                        column: x => x.InstrutorPessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoInstrutor_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventoInstrutor_SolucoesEducacionais_SolucaoEducacionalId",
                        column: x => x.SolucaoEducacionalId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_GEDTHPessoaId",
                table: "Evento",
                column: "GEDTHPessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_EntidadeDemandanteId",
                table: "Evento",
                column: "EntidadeDemandanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_LocalId",
                table: "Evento",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_SolucaoEducacionalId",
                table: "Evento",
                column: "SolucaoEducacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoAgenda_EventoId",
                table: "EventoAgenda",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoCota_EntidadeId",
                table: "EventoCota",
                column: "EntidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoCota_EventoId",
                table: "EventoCota",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_DocenteId",
                table: "EventoHorario",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_FuncaoId",
                table: "EventoHorario",
                column: "FuncaoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_LocalId",
                table: "EventoHorario",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_LocalSalaId",
                table: "EventoHorario",
                column: "LocalSalaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_ModuloId",
                table: "EventoHorario",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_SolucaoEducacionalId",
                table: "EventoHorario",
                column: "SolucaoEducacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInstrutor_EventoId",
                table: "EventoInstrutor",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInstrutor_FuncaoId",
                table: "EventoInstrutor",
                column: "FuncaoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInstrutor_InstrutorPessoaId",
                table: "EventoInstrutor",
                column: "InstrutorPessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInstrutor_ModuloId",
                table: "EventoInstrutor",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInstrutor_SolucaoEducacionalId",
                table: "EventoInstrutor",
                column: "SolucaoEducacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoRecurso_EventoId",
                table: "EventoRecurso",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoRecurso_RecursoId",
                table: "EventoRecurso",
                column: "RecursoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoAgenda");

            migrationBuilder.DropTable(
                name: "EventoCota");

            migrationBuilder.DropTable(
                name: "EventoHorario");

            migrationBuilder.DropTable(
                name: "EventoInstrutor");

            migrationBuilder.DropTable(
                name: "EventoRecurso");

            migrationBuilder.DropTable(
                name: "Docente");

            migrationBuilder.DropTable(
                name: "Funcao");

            migrationBuilder.DropTable(
                name: "Evento");
        }
    }
}
