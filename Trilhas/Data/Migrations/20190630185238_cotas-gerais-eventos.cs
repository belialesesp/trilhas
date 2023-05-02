using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class cotasgeraiseventos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NaoLimitarInscricoes",
                table: "Eventos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VagasPorEntidade",
                table: "Eventos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NaoLimitarInscricoes",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "VagasPorEntidade",
                table: "Eventos");
        }
    }
}
