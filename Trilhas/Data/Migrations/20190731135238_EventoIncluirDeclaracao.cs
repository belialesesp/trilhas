using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class EventoIncluirDeclaracao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeclaracaoCursistaId",
                table: "Eventos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeclaracaoDocenteId",
                table: "Eventos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_DeclaracaoCursistaId",
                table: "Eventos",
                column: "DeclaracaoCursistaId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_DeclaracaoDocenteId",
                table: "Eventos",
                column: "DeclaracaoDocenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Certificados_DeclaracaoCursistaId",
                table: "Eventos",
                column: "DeclaracaoCursistaId",
                principalTable: "Certificados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Certificados_DeclaracaoDocenteId",
                table: "Eventos",
                column: "DeclaracaoDocenteId",
                principalTable: "Certificados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Certificados_DeclaracaoCursistaId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Certificados_DeclaracaoDocenteId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_DeclaracaoCursistaId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_DeclaracaoDocenteId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DeclaracaoCursistaId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DeclaracaoDocenteId",
                table: "Eventos");
        }
    }
}
