using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracoescurso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_Entidades_EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.AlterColumn<decimal>(
                name: "Custo",
                table: "Recursos",
                type: "decimal(7, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EntidadeId",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Custo",
                table: "Recursos",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_EntidadeId",
                table: "SolucoesEducacionais",
                column: "EntidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_Entidades_EntidadeId",
                table: "SolucoesEducacionais",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
