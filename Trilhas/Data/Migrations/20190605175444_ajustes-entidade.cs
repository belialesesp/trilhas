using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustesentidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades");

            migrationBuilder.AlterColumn<long>(
                name: "TipoId",
                table: "Entidades",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TipoDeEntidadeId",
                table: "Entidades",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades",
                column: "TipoId",
                principalTable: "TiposDeEntidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "TipoDeEntidadeId",
                table: "Entidades");

            migrationBuilder.AlterColumn<long>(
                name: "TipoId",
                table: "Entidades",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Entidades_TiposDeEntidade_TipoId",
                table: "Entidades",
                column: "TipoId",
                principalTable: "TiposDeEntidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
