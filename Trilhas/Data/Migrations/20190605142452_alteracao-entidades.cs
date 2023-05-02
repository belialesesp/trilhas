using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaoentidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Entidades",
                newName: "Sigla");

            migrationBuilder.AlterColumn<decimal>(
                name: "Custo",
                table: "Recursos",
                type: "decimal(7, 2)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sigla",
                table: "Entidades",
                newName: "Descricao");

            migrationBuilder.AlterColumn<decimal>(
                name: "Custo",
                table: "Recursos",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7, 2)");
        }
    }
}
