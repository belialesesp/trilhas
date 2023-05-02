using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class Modalidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ModalidadeId",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Modalidade",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modalidade", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Modalidade",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 1L, "EAD" });

            migrationBuilder.InsertData(
                table: "Modalidade",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 2L, "Presencial" });

            migrationBuilder.InsertData(
                table: "Modalidade",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 3L, "SemiPresencial" });

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_ModalidadeId",
                table: "SolucoesEducacionais",
                column: "ModalidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_Modalidade_ModalidadeId",
                table: "SolucoesEducacionais",
                column: "ModalidadeId",
                principalTable: "Modalidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_Modalidade_ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropTable(
                name: "Modalidade");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.InsertData(
                table: "TiposDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 1L, "PRESENCIAL" });

            migrationBuilder.InsertData(
                table: "TiposDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 3L, "EAD" });
        }
    }
}
