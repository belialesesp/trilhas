using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class seednivelcurso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NiveisDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 1L, "Capacitação" });

            migrationBuilder.InsertData(
                table: "NiveisDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 2L, "Formação" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NiveisDeCurso",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "NiveisDeCurso",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
