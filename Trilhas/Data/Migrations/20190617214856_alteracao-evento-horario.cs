using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaoeventohorario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Locais_LocalId",
                table: "EventoHorario");

            migrationBuilder.DropIndex(
                name: "IX_EventoHorario_LocalId",
                table: "EventoHorario");

            migrationBuilder.DropColumn(
                name: "LocalId",
                table: "EventoHorario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocalId",
                table: "EventoHorario",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoHorario_LocalId",
                table: "EventoHorario",
                column: "LocalId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Locais_LocalId",
                table: "EventoHorario",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
