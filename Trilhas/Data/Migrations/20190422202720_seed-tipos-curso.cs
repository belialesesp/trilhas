using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class seedtiposcurso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1L, "EaD" },
                    { 2L, "Fórum" },
                    { 3L, "Mesa Redonda" },
                    { 4L, "Palestra" },
                    { 5L, "Seminário" },
                    { 6L, "WorkShop" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 6L);
        }
    }
}
