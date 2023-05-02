using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustemodelosolucao3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Livro_Titulo",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "Video_Titulo",
                table: "SolucoesEducacionais");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Livro_Titulo",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Titulo",
                table: "SolucoesEducacionais",
                nullable: true);
        }
    }
}
