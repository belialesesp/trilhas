using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustemodelosolucao4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tipo_solucao",
                table: "SolucoesEducacionais",
                newName: "TipoDeSolucao");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoDeSolucao",
                table: "SolucoesEducacionais",
                newName: "tipo_solucao");
        }
    }
}
