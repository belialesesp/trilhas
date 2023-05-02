using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class locaismunicipio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Locais");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "Locais");

            migrationBuilder.AddColumn<long>(
                name: "MunicipioId",
                table: "Locais",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locais_MunicipioId",
                table: "Locais",
                column: "MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locais_Municipios_MunicipioId",
                table: "Locais",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locais_Municipios_MunicipioId",
                table: "Locais");

            migrationBuilder.DropIndex(
                name: "IX_Locais_MunicipioId",
                table: "Locais");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "Locais");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Locais",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "Locais",
                nullable: true);
        }
    }
}
