using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class gestor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDeEntidadeId",
                table: "Entidades");

            migrationBuilder.CreateTable(
                name: "Gestor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PessoaId = table.Column<long>(nullable: true),
                    EntidadeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gestor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gestor_Entidades_EntidadeId",
                        column: x => x.EntidadeId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gestor_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gestor_EntidadeId",
                table: "Gestor",
                column: "EntidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Gestor_PessoaId",
                table: "Gestor",
                column: "PessoaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gestor");

            migrationBuilder.AddColumn<long>(
                name: "TipoDeEntidadeId",
                table: "Entidades",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
