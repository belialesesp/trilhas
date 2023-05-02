using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaoimagemeixo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eixos_ArquivoExterno_ImagemId",
                table: "Eixos");

            migrationBuilder.DropTable(
                name: "ArquivoExterno");

            migrationBuilder.DropIndex(
                name: "IX_Eixos_ImagemId",
                table: "Eixos");

            migrationBuilder.DropColumn(
                name: "ImagemId",
                table: "Eixos");

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Eixos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Eixos");

            migrationBuilder.AddColumn<long>(
                name: "ImagemId",
                table: "Eixos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArquivoExterno",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Extensao = table.Column<string>(nullable: true),
                    Identificador = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArquivoExterno", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eixos_ImagemId",
                table: "Eixos",
                column: "ImagemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eixos_ArquivoExterno_ImagemId",
                table: "Eixos",
                column: "ImagemId",
                principalTable: "ArquivoExterno",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
