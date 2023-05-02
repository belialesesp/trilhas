using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class eventocertificado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CertificadoId",
                table: "Eventos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_CertificadoId",
                table: "Eventos",
                column: "CertificadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Certificados_CertificadoId",
                table: "Eventos",
                column: "CertificadoId",
                principalTable: "Certificados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Certificados_CertificadoId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_CertificadoId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CertificadoId",
                table: "Eventos");
        }
    }
}
