using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class TabelasAuxiliares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PessoaContato_TipoPessoaContato_TipoContatoId",
                table: "PessoaContato");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "PessoaContato");

            migrationBuilder.RenameColumn(
                name: "TipoContatoId",
                table: "PessoaContato",
                newName: "TipoPessoaContatoId");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaContato_TipoContatoId",
                table: "PessoaContato",
                newName: "IX_PessoaContato_TipoPessoaContatoId");

            migrationBuilder.RenameColumn(
                name: "OrgaoExpedidorIdentidade",
                table: "Pessoa",
                newName: "Email");

            migrationBuilder.AddColumn<long>(
                name: "EntidadeId",
                table: "Pessoa",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrgaoExpedidorId",
                table: "Pessoa",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SexoId",
                table: "Pessoa",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrgaoExpedidor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Sigla = table.Column<string>(nullable: true),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgaoExpedidor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_EntidadeId",
                table: "Pessoa",
                column: "EntidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_OrgaoExpedidorId",
                table: "Pessoa",
                column: "OrgaoExpedidorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoa_SexoId",
                table: "Pessoa",
                column: "SexoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_Entidades_EntidadeId",
                table: "Pessoa",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_OrgaoExpedidor_OrgaoExpedidorId",
                table: "Pessoa",
                column: "OrgaoExpedidorId",
                principalTable: "OrgaoExpedidor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_Sexo_SexoId",
                table: "Pessoa",
                column: "SexoId",
                principalTable: "Sexo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaContato_TipoPessoaContato_TipoPessoaContatoId",
                table: "PessoaContato",
                column: "TipoPessoaContatoId",
                principalTable: "TipoPessoaContato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Entidades_EntidadeId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_OrgaoExpedidor_OrgaoExpedidorId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Sexo_SexoId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaContato_TipoPessoaContato_TipoPessoaContatoId",
                table: "PessoaContato");

            migrationBuilder.DropTable(
                name: "OrgaoExpedidor");

            migrationBuilder.DropTable(
                name: "Sexo");

            migrationBuilder.DropIndex(
                name: "IX_Pessoa_EntidadeId",
                table: "Pessoa");

            migrationBuilder.DropIndex(
                name: "IX_Pessoa_OrgaoExpedidorId",
                table: "Pessoa");

            migrationBuilder.DropIndex(
                name: "IX_Pessoa_SexoId",
                table: "Pessoa");

            migrationBuilder.DropColumn(
                name: "EntidadeId",
                table: "Pessoa");

            migrationBuilder.DropColumn(
                name: "OrgaoExpedidorId",
                table: "Pessoa");

            migrationBuilder.DropColumn(
                name: "SexoId",
                table: "Pessoa");

            migrationBuilder.RenameColumn(
                name: "TipoPessoaContatoId",
                table: "PessoaContato",
                newName: "TipoContatoId");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaContato_TipoPessoaContatoId",
                table: "PessoaContato",
                newName: "IX_PessoaContato_TipoContatoId");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Pessoa",
                newName: "OrgaoExpedidorIdentidade");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "PessoaContato",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaContato_TipoPessoaContato_TipoContatoId",
                table: "PessoaContato",
                column: "TipoContatoId",
                principalTable: "TipoPessoaContato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
