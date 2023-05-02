using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class mudancamodalidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modalidade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                values: new object[] { 2L, "PRESENCIAL" });

            migrationBuilder.InsertData(
                table: "Modalidade",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 3L, "SEMIPRESENCIAL" });
        }
    }
}
