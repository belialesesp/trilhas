using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class trilhasdousuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trilhas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<string>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DeletionUserId = table.Column<string>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trilhas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensDaTrilha",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<string>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DeletionUserId = table.Column<string>(nullable: true),
                    TrilhaId = table.Column<long>(nullable: true),
                    SolucaoId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensDaTrilha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensDaTrilha_SolucoesEducacionais_SolucaoId",
                        column: x => x.SolucaoId,
                        principalTable: "SolucoesEducacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensDaTrilha_Trilhas_TrilhaId",
                        column: x => x.TrilhaId,
                        principalTable: "Trilhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensDaTrilha_SolucaoId",
                table: "ItensDaTrilha",
                column: "SolucaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensDaTrilha_TrilhaId",
                table: "ItensDaTrilha",
                column: "TrilhaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensDaTrilha");

            migrationBuilder.DropTable(
                name: "Trilhas");
        }
    }
}
