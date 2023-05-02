using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class seedtipoentidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Descricao",
                value: "Municipal");

            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Descricao",
                value: "Estadual");

            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "Federal");

            migrationBuilder.InsertData(
                table: "TiposDeEntidade",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 4L, "Organização Não Governamental" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Descricao",
                value: "Estadual");

            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Descricao",
                value: "Municipal");

            migrationBuilder.UpdateData(
                table: "TiposDeEntidade",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "Entidade Não Governamental");
        }
    }
}
