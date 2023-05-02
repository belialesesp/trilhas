using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class pessoa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deficiencia",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deficiencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Escolaridade",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escolaridade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoPessoaContato",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoPessoaContato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoa",
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
                    Cpf = table.Column<string>(nullable: true),
                    NumeroFuncional = table.Column<string>(nullable: true),
                    NomeSocial = table.Column<string>(nullable: true),
                    DataNascimento = table.Column<DateTime>(nullable: false),
                    NumeroIdentidade = table.Column<string>(nullable: true),
                    OrgaoExpedidorIdentidade = table.Column<string>(nullable: true),
                    UfIdentidade = table.Column<string>(nullable: true),
                    NumeroTitulo = table.Column<string>(nullable: true),
                    SecaoTitulo = table.Column<string>(nullable: true),
                    ZonaTitulo = table.Column<string>(nullable: true),
                    Pis = table.Column<string>(nullable: true),
                    FlagDeficiente = table.Column<bool>(nullable: false),
                    DeficienciaId = table.Column<long>(nullable: true),
                    EscolaridadeId = table.Column<long>(nullable: true),
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
                    table.PrimaryKey("PK_Pessoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pessoa_Deficiencia_DeficienciaId",
                        column: x => x.DeficienciaId,
                        principalTable: "Deficiencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pessoa_Escolaridade_EscolaridadeId",
                        column: x => x.EscolaridadeId,
                        principalTable: "Escolaridade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PessoaContato",
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
                    Numero = table.Column<string>(nullable: true),
                    TipoContatoId = table.Column<long>(nullable: true),
                    PessoaId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoaContato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PessoaContato_Pessoa_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PessoaContato_TipoPessoaContato_TipoContatoId",
                        column: x => x.TipoContatoId,
                        principalTable: "TipoPessoaContato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_DeficienciaId",
                table: "Pessoa",
                column: "DeficienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_EscolaridadeId",
                table: "Pessoa",
                column: "EscolaridadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaContato_PessoaId",
                table: "PessoaContato",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaContato_TipoContatoId",
                table: "PessoaContato",
                column: "TipoContatoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PessoaContato");

            migrationBuilder.DropTable(
                name: "Pessoa");

            migrationBuilder.DropTable(
                name: "TipoPessoaContato");

            migrationBuilder.DropTable(
                name: "Deficiencia");

            migrationBuilder.DropTable(
                name: "Escolaridade");
        }
    }
}
