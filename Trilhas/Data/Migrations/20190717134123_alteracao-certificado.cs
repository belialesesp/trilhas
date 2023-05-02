using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaocertificado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FlagConteudoProgramatico",
                table: "Certificados",
                newName: "Padrao");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Padrao",
                table: "Certificados",
                newName: "FlagConteudoProgramatico");
        }
    }
}
