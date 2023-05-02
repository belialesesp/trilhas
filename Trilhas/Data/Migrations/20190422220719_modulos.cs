using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class modulos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.CreateTable(
                name: "Modulo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolucaoEducacionalId = table.Column<long>(nullable: false),
                    Nome = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: true),
                    CargaHoraria = table.Column<int>(nullable: false),
                    CursoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modulo_SolucoesEducacionais_CursoId",
                        column: x => x.CursoId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Descricao",
                value: "Presencial");

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "EaD");

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Descricao",
                value: "Fórum");

            migrationBuilder.InsertData(
                table: "TiposDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 7L, "Mesa Redonda" },
                    { 9L, "Palestra" },
                    { 11L, "Seminário" },
                    { 13L, "WorkShop" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modulo_CursoId",
                table: "Modulo",
                column: "CursoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modulo");

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Descricao",
                value: "EaD");

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Descricao",
                value: "Mesa Redonda");

            migrationBuilder.UpdateData(
                table: "TiposDeCurso",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Descricao",
                value: "Seminário");

            migrationBuilder.InsertData(
                table: "TiposDeCurso",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 2L, "Fórum" },
                    { 4L, "Palestra" },
                    { 6L, "WorkShop" }
                });
        }
    }
}
