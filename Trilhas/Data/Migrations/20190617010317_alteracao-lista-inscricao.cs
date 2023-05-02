using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class alteracaolistainscricao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ListaDeInscricao",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorUserId",
                table: "ListaDeInscricao",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "ListaDeInscricao",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletionUserId",
                table: "ListaDeInscricao",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ListaDeInscricao",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierUserId",
                table: "ListaDeInscricao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ListaDeInscricao");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "ListaDeInscricao");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "ListaDeInscricao");

            migrationBuilder.DropColumn(
                name: "DeletionUserId",
                table: "ListaDeInscricao");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ListaDeInscricao");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "ListaDeInscricao");
        }
    }
}
