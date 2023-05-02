using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class remocaosituacaoevento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Situacao",
                table: "Eventos");

            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Eventos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Eventos");

            migrationBuilder.AddColumn<int>(
                name: "Situacao",
                table: "Eventos",
                nullable: false,
                defaultValue: 0);
        }
    }
}
