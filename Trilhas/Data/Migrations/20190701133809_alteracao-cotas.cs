using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaocotas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NaoLimitarInscricoes",
                table: "Eventos",
                newName: "LimitarInscricoes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LimitarInscricoes",
                table: "Eventos",
                newName: "NaoLimitarInscricoes");
        }
    }
}
