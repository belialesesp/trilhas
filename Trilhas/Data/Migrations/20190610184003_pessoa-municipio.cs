using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class pessoamunicipio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilidades_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Habilidades");

            migrationBuilder.DropIndex(
                name: "IX_Habilidades_SolucaoEducacionalId",
                table: "Habilidades");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "SolucaoEducacionalId",
                table: "Habilidades");

            migrationBuilder.AddColumn<long>(
                name: "MunicipioId",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_MunicipioId",
                table: "Pessoas",
                column: "MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Municipios_MunicipioId",
                table: "Pessoas",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Municipios_MunicipioId",
                table: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_MunicipioId",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "Pessoas");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Habilidades",
                nullable: true);

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
    }
}
