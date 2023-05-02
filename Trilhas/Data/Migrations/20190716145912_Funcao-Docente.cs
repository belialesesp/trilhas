using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class FuncaoDocente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Funcao",
                table: "EventoHorario");

            migrationBuilder.AddColumn<long>(
                name: "FuncaoId",
                table: "EventoHorario",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FuncoesDocente",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncoesDocente", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FuncoesDocente",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1L, "DOCENTE" },
                    { 2L, "DOCENTE ASSISTENTE" },
                    { 3L, "CONFERENCISTA" },
                    { 4L, "PALESTRANTE" },
                    { 5L, "PAINELISTA" },
                    { 6L, "DEBATEDOR" },
                    { 7L, "MODERADOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_FuncaoId",
                table: "EventoHorario",
                column: "FuncaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_FuncoesDocente_FuncaoId",
                table: "EventoHorario",
                column: "FuncaoId",
                principalTable: "FuncoesDocente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_FuncoesDocente_FuncaoId",
                table: "EventoHorario");

            migrationBuilder.DropTable(
                name: "FuncoesDocente");

            migrationBuilder.DropIndex(
                name: "IX_EventoHorario_FuncaoId",
                table: "EventoHorario");

            migrationBuilder.DropColumn(
                name: "FuncaoId",
                table: "EventoHorario");

            migrationBuilder.AddColumn<string>(
                name: "Funcao",
                table: "EventoHorario",
                nullable: true);
        }
    }
}
