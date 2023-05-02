using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class EnumModalidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Modulos");

            migrationBuilder.DropForeignKey(
                name: "FK_SolucoesEducacionais_Modalidade_ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_SolucoesEducacionais_ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropIndex(
                name: "IX_Modulos_SolucaoEducacionalId",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "ModalidadeId",
                table: "SolucoesEducacionais");

            migrationBuilder.DropColumn(
                name: "SolucaoEducacionalId",
                table: "Modulos");

            migrationBuilder.AddColumn<int>(
                name: "Modalidade",
                table: "SolucoesEducacionais",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modalidade",
                table: "SolucoesEducacionais");

            migrationBuilder.AddColumn<long>(
                name: "ModalidadeId",
                table: "SolucoesEducacionais",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SolucaoEducacionalId",
                table: "Modulos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolucoesEducacionais_ModalidadeId",
                table: "SolucoesEducacionais",
                column: "ModalidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_SolucaoEducacionalId",
                table: "Modulos",
                column: "SolucaoEducacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulos_SolucoesEducacionais_SolucaoEducacionalId",
                table: "Modulos",
                column: "SolucaoEducacionalId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolucoesEducacionais_Modalidade_ModalidadeId",
                table: "SolucoesEducacionais",
                column: "ModalidadeId",
                principalTable: "Modalidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
