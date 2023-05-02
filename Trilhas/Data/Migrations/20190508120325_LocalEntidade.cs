using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class LocalEntidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Local",
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
                    Nome = table.Column<string>(nullable: true),
                    Observacoes = table.Column<string>(nullable: true),
                    Cep = table.Column<string>(nullable: true),
                    Logradouro = table.Column<string>(nullable: true),
                    Bairro = table.Column<string>(nullable: true),
                    Numero = table.Column<string>(nullable: true),
                    Complemento = table.Column<string>(nullable: true),
                    Cidade = table.Column<string>(nullable: true),
                    Uf = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Local", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoLocalContato",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoLocalContato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalRecursos",
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
                    LocalId = table.Column<long>(nullable: true),
                    RecursoId = table.Column<long>(nullable: true),
                    Quantidade = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalRecursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalRecursos_Local_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Local",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalRecursos_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalSalas",
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
                    LocalId = table.Column<long>(nullable: true),
                    Sigla = table.Column<string>(nullable: true),
                    Numero = table.Column<string>(nullable: true),
                    Capacidade = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalSalas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalSalas_Local_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Local",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalContatos",
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
                    LocalId = table.Column<long>(nullable: true),
                    TipoContatoId = table.Column<long>(nullable: true),
                    NumeroTelefone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalContatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalContatos_Local_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Local",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalContatos_TipoLocalContato_TipoContatoId",
                        column: x => x.TipoContatoId,
                        principalTable: "TipoLocalContato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalContatos_LocalId",
                table: "LocalContatos",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalContatos_TipoContatoId",
                table: "LocalContatos",
                column: "TipoContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalRecursos_LocalId",
                table: "LocalRecursos",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalRecursos_RecursoId",
                table: "LocalRecursos",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalSalas_LocalId",
                table: "LocalSalas",
                column: "LocalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalContatos");

            migrationBuilder.DropTable(
                name: "LocalRecursos");

            migrationBuilder.DropTable(
                name: "LocalSalas");

            migrationBuilder.DropTable(
                name: "TipoLocalContato");

            migrationBuilder.DropTable(
                name: "Local");
        }
    }
}
