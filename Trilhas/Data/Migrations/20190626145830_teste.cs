using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class teste : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipoPessoaContato",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DropColumn(
                name: "SecaoTitulo",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "ZonaTitulo",
                table: "Pessoas");

            migrationBuilder.UpdateData(
                table: "Modalidade",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Descricao",
                value: "PRESENCIAL");

            migrationBuilder.UpdateData(
                table: "Modalidade",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "SEMIPRESENCIAL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecaoTitulo",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZonaTitulo",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Modalidade",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Descricao",
                value: "Presencial");

            migrationBuilder.UpdateData(
                table: "Modalidade",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "SemiPresencial");

            migrationBuilder.InsertData(
                table: "TipoPessoaContato",
                columns: new[] { "Id", "Nome" },
                values: new object[] { 4L, "s" });
        }
    }
}
