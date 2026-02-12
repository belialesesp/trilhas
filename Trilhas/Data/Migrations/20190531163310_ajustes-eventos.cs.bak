using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajusteseventos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evento_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Funcao_FuncaoId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_SolucoesEducacionais_SolucaoEducacionalId",
                table: "EventoHorario");

            migrationBuilder.DropTable(
                name: "EventoInstrutor");

            migrationBuilder.DropTable(
                name: "Funcao");

            migrationBuilder.DropIndex(
                name: "IX_EventoHorario_FuncaoId",
                table: "EventoHorario");

            migrationBuilder.DropColumn(
                name: "FuncaoId",
                table: "EventoHorario");

            migrationBuilder.DropColumn(
                name: "FlagEad",
                table: "Evento");

            migrationBuilder.RenameColumn(
                name: "SolucaoEducacionalId",
                table: "EventoHorario",
                newName: "EventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoHorario_SolucaoEducacionalId",
                table: "EventoHorario",
                newName: "IX_EventoHorario_EventoId");

            migrationBuilder.RenameColumn(
                name: "SolucaoEducacionalId",
                table: "Evento",
                newName: "CursoId");

            migrationBuilder.RenameColumn(
                name: "Observacao",
                table: "Evento",
                newName: "Observacoes");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_SolucaoEducacionalId",
                table: "Evento",
                newName: "IX_Evento_CursoId");

            migrationBuilder.AddColumn<string>(
                name: "Funcao",
                table: "EventoHorario",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Deficiencia",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1L, "Física" },
                    { 2L, "Auditiva" },
                    { 3L, "Visual" },
                    { 4L, "Mental" }
                });

            migrationBuilder.InsertData(
                table: "Escolaridade",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1L, "1º Grau Incompleto" },
                    { 2L, "1º Grau Completo" },
                    { 3L, "Ensino Superior Incompleto" },
                    { 4L, "Ensino Superior Completo" }
                });

            migrationBuilder.InsertData(
                table: "OrgaoExpedidor",
                columns: new[] { "Id", "Nome", "Sigla" },
                values: new object[] { 1L, "Secretaria de Segurança Pública", "SSP" });

            migrationBuilder.InsertData(
                table: "Sexo",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1L, "Masculino" },
                    { 2L, "Feminino" },
                    { 3L, "Outro" }
                });

            migrationBuilder.InsertData(
                table: "TipoLocalContato",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1L, "Fixo" },
                    { 2L, "Celular" },
                    { 3L, "Fax" }
                });

            migrationBuilder.InsertData(
                table: "TipoPessoaContato",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1L, "Fixo" },
                    { 2L, "Celular" },
                    { 3L, "Fax" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_SolucoesEducacionais_CursoId",
                table: "Evento",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Evento_EventoId",
                table: "EventoHorario",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evento_SolucoesEducacionais_CursoId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Evento_EventoId",
                table: "EventoHorario");

            migrationBuilder.DeleteData(
                table: "Deficiencia",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Deficiencia",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Deficiencia",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Deficiencia",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Escolaridade",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Escolaridade",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Escolaridade",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Escolaridade",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "OrgaoExpedidor",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Sexo",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Sexo",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Sexo",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "TipoLocalContato",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "TipoLocalContato",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "TipoLocalContato",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "TipoPessoaContato",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "TipoPessoaContato",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "TipoPessoaContato",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DropColumn(
                name: "Funcao",
                table: "EventoHorario");

            migrationBuilder.RenameColumn(
                name: "EventoId",
                table: "EventoHorario",
                newName: "SolucaoEducacionalId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoHorario_EventoId",
                table: "EventoHorario",
                newName: "IX_EventoHorario_SolucaoEducacionalId");

            migrationBuilder.RenameColumn(
                name: "Observacoes",
                table: "Evento",
                newName: "Observacao");

            migrationBuilder.RenameColumn(
                name: "CursoId",
                table: "Evento",
                newName: "SolucaoEducacionalId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_CursoId",
                table: "Evento",
                newName: "IX_Evento_SolucaoEducacionalId");

            migrationBuilder.AddColumn<long>(
                name: "FuncaoId",
                table: "EventoHorario",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FlagEad",
                table: "Evento",
                nullable: false,
                defaultValue: false);

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
                name: "EventoInstrutor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataHoraFim = table.Column<DateTime>(nullable: false),
                    DataHoraInicio = table.Column<DateTime>(nullable: false),
                    EventoId = table.Column<long>(nullable: true),
                    FuncaoId = table.Column<long>(nullable: true),
                    InstrutorPessoaId = table.Column<long>(nullable: true),
                    ModuloId = table.Column<long>(nullable: true),
                    SolucaoEducacionalId = table.Column<long>(nullable: true)
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
                name: "IX_EventoHorario_FuncaoId",
                table: "EventoHorario",
                column: "FuncaoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Evento",
                column: "SolucaoEducacionalId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Funcao_FuncaoId",
                table: "EventoHorario",
                column: "FuncaoId",
                principalTable: "Funcao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_SolucoesEducacionais_SolucaoEducacionalId",
                table: "EventoHorario",
                column: "SolucaoEducacionalId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
