using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ListaDeInscricao_Inscricao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListaDeInscricao",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaDeInscricao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaDeInscricao_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inscricao",
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
                    ListaDeInscricaoId = table.Column<long>(nullable: true)
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
                        name: "FK_Inscricao_Pessoa_PessoaId",
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
                name: "IX_ListaDeInscricao_EventoId",
                table: "ListaDeInscricao",
                column: "EventoId",
                unique: true,
                filter: "[EventoId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inscricao");

            migrationBuilder.DropTable(
                name: "ListaDeInscricao");
        }
    }
}
