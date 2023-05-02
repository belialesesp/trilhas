using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ListaPresenca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipoPessoaContato",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.CreateTable(
                name: "ListaPresenca",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventoHorarioId = table.Column<long>(nullable: true),
                    PessoaId = table.Column<long>(nullable: true),
                    Presente = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaPresenca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaPresenca_EventoHorario_EventoHorarioId",
                        column: x => x.EventoHorarioId,
                        principalTable: "EventoHorario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListaPresenca_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresenca_EventoHorarioId",
                table: "ListaPresenca",
                column: "EventoHorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresenca_PessoaId",
                table: "ListaPresenca",
                column: "PessoaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListaPresenca");

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
