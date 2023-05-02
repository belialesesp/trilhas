using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Habilidades",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_Habilidades_SolucaoEducacionalId",
                table: "Habilidades",
                column: "SolucaoEducacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Habilidades",
                column: "SolucaoEducacionalId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Habilidades");

            migrationBuilder.DropIndex(
                name: "IX_Habilidades_SolucaoEducacionalId",
                table: "Habilidades");

            migrationBuilder.AlterColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Habilidades",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
