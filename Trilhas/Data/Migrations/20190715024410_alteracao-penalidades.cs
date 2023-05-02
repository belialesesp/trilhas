using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaopenalidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataDaPenalidade",
                table: "Penalidades",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JustificativaDeCancelamento",
                table: "Penalidades",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PenalidadeId",
                table: "Inscritos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscritos_PenalidadeId",
                table: "Inscritos",
                column: "PenalidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscritos_Penalidades_PenalidadeId",
                table: "Inscritos",
                column: "PenalidadeId",
                principalTable: "Penalidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscritos_Penalidades_PenalidadeId",
                table: "Inscritos");

            migrationBuilder.DropIndex(
                name: "IX_Inscritos_PenalidadeId",
                table: "Inscritos");

            migrationBuilder.DropColumn(
                name: "DataDaPenalidade",
                table: "Penalidades");

            migrationBuilder.DropColumn(
                name: "JustificativaDeCancelamento",
                table: "Penalidades");

            migrationBuilder.DropColumn(
                name: "PenalidadeId",
                table: "Inscritos");
        }
    }
}
