using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class renamefrequencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PercentualDeFrequencia",
                table: "Inscritos",
                newName: "Frequencia");

            migrationBuilder.AddColumn<int>(
                name: "Situacao",
                table: "Inscritos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Situacao",
                table: "Inscritos");

            migrationBuilder.RenameColumn(
                name: "Frequencia",
                table: "Inscritos",
                newName: "PercentualDeFrequencia");
        }
    }
}
