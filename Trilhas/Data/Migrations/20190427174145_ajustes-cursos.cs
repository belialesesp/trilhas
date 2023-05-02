using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustescursos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gestor",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "Municipio",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "UF",
                table: "Entidades");

            migrationBuilder.AddColumn<long>(
                name: "EntidadeId",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MunicipioId",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TipoId",
                table: "Entidades",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    codigoMunicipio = table.Column<long>(nullable: false),
                    NomeMunicipio = table.Column<string>(nullable: true),
                    codigoUf = table.Column<int>(nullable: false),
                    Uf = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDeEntidade",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDeEntidade", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Municipios",
                columns: new[] { "Id", "NomeMunicipio", "Uf", "codigoMunicipio", "codigoUf" },
                values: new object[,]
                {
                    { 1L, "Vitória", "ES", 3205309L, 32 },
                    { 2L, "Vila Velha", "ES", 3205200L, 32 },
                    { 3L, "Serra", "ES", 3205002L, 32 },
                    { 4L, "Cariacica", "ES", 3201308L, 32 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_EntidadeId",
                table: "SolucoesEducacionais",
                column: "EntidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entidades_MunicipioId",
                table: "Entidades",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Entidades_TipoId",
                table: "Entidades",
                column: "TipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entidades_Municipios_MunicipioId",
                table: "Entidades",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades",
                column: "TipoId",
                principalTable: "TiposDeEntidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_Entidades_EntidadeId",
                table: "SolucoesEducacionais",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entidades_Municipios_MunicipioId",
                table: "Entidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_Entidades_EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "TiposDeEntidade");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_Entidades_MunicipioId",
                table: "Entidades");

            migrationBuilder.DropIndex(
                name: "IX_Entidades_TipoId",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "EntidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "TipoId",
                table: "Entidades");

            migrationBuilder.AddColumn<string>(
                name: "Gestor",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipio",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UF",
                table: "Entidades",
                nullable: true);
        }
    }
}
