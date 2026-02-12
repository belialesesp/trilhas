using Microsoft.EntityFrameworkCore.Migrations;

namespace Trilhas.Data.Migrations
{
    public partial class ajustesgestores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Docente_Pessoa_PessoaId",
                table: "Docente");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Pessoa_GEDTHPessoaId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_SolucoesEducacionais_CursoId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Entidades_EntidadeDemandanteId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Local_LocalId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoAgenda_Evento_EventoId",
                table: "EventoAgenda");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoCota_Evento_EventoId",
                table: "EventoCota");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Docente_DocenteId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Evento_EventoId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Local_LocalId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoRecurso_Evento_EventoId",
                table: "EventoRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestor_Entidades_EntidadeId",
                table: "Gestor");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestor_Pessoa_PessoaId",
                table: "Gestor");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalContatos_Local_LocalId",
                table: "LocalContatos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalRecursos_Local_LocalId",
                table: "LocalRecursos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalSalas_Local_LocalId",
                table: "LocalSalas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Deficiencia_DeficienciaId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Entidades_EntidadeId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Escolaridade_EscolaridadeId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_OrgaoExpedidor_OrgaoExpedidorId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoa_Sexo_SexoId",
                table: "Pessoa");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaContato_Pessoa_PessoaId",
                table: "PessoaContato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pessoa",
                table: "Pessoa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Local",
                table: "Local");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gestor",
                table: "Gestor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evento",
                table: "Evento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Escolaridade",
                table: "Escolaridade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Docente",
                table: "Docente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deficiencia",
                table: "Deficiencia");

            migrationBuilder.RenameTable(
                name: "Pessoa",
                newName: "Pessoas");

            migrationBuilder.RenameTable(
                name: "Local",
                newName: "Locais");

            migrationBuilder.RenameTable(
                name: "Gestor",
                newName: "Gestores");

            migrationBuilder.RenameTable(
                name: "Evento",
                newName: "Eventos");

            migrationBuilder.RenameTable(
                name: "Escolaridade",
                newName: "Escolaridades");

            migrationBuilder.RenameTable(
                name: "Docente",
                newName: "Docentes");

            migrationBuilder.RenameTable(
                name: "Deficiencia",
                newName: "Deficiencias");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoa_SexoId",
                table: "Pessoas",
                newName: "IX_Pessoas_SexoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoa_OrgaoExpedidorId",
                table: "Pessoas",
                newName: "IX_Pessoas_OrgaoExpedidorId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoa_EscolaridadeId",
                table: "Pessoas",
                newName: "IX_Pessoas_EscolaridadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoa_EntidadeId",
                table: "Pessoas",
                newName: "IX_Pessoas_EntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoa_DeficienciaId",
                table: "Pessoas",
                newName: "IX_Pessoas_DeficienciaId");

            migrationBuilder.RenameIndex(
                name: "IX_Gestor_PessoaId",
                table: "Gestores",
                newName: "IX_Gestores_PessoaId");

            migrationBuilder.RenameIndex(
                name: "IX_Gestor_EntidadeId",
                table: "Gestores",
                newName: "IX_Gestores_EntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_LocalId",
                table: "Eventos",
                newName: "IX_Eventos_LocalId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_EntidadeDemandanteId",
                table: "Eventos",
                newName: "IX_Eventos_EntidadeDemandanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_CursoId",
                table: "Eventos",
                newName: "IX_Eventos_CursoId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_GEDTHPessoaId",
                table: "Eventos",
                newName: "IX_Eventos_GEDTHPessoaId");

            migrationBuilder.RenameIndex(
                name: "IX_Docente_PessoaId",
                table: "Docentes",
                newName: "IX_Docentes_PessoaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locais",
                table: "Locais",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gestores",
                table: "Gestores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Escolaridades",
                table: "Escolaridades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Docentes",
                table: "Docentes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deficiencias",
                table: "Deficiencias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Docentes_Pessoas_PessoaId",
                table: "Docentes",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoAgenda_Eventos_EventoId",
                table: "EventoAgenda",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoCota_Eventos_EventoId",
                table: "EventoCota",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Docentes_DocenteId",
                table: "EventoHorario",
                column: "DocenteId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Eventos_EventoId",
                table: "EventoHorario",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Locais_LocalId",
                table: "EventoHorario",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoRecurso_Eventos_EventoId",
                table: "EventoRecurso",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Pessoas_GEDTHPessoaId",
                table: "Eventos",
                column: "GEDTHPessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_SolucoesEducacionais_CursoId",
                table: "Eventos",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Entidades_EntidadeDemandanteId",
                table: "Eventos",
                column: "EntidadeDemandanteId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Locais_LocalId",
                table: "Eventos",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestores_Entidades_EntidadeId",
                table: "Gestores",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestores_Pessoas_PessoaId",
                table: "Gestores",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalContatos_Locais_LocalId",
                table: "LocalContatos",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalRecursos_Locais_LocalId",
                table: "LocalRecursos",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalSalas_Locais_LocalId",
                table: "LocalSalas",
                column: "LocalId",
                principalTable: "Locais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaContato_Pessoas_PessoaId",
                table: "PessoaContato",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Deficiencias_DeficienciaId",
                table: "Pessoas",
                column: "DeficienciaId",
                principalTable: "Deficiencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Entidades_EntidadeId",
                table: "Pessoas",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Escolaridades_EscolaridadeId",
                table: "Pessoas",
                column: "EscolaridadeId",
                principalTable: "Escolaridades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_OrgaoExpedidor_OrgaoExpedidorId",
                table: "Pessoas",
                column: "OrgaoExpedidorId",
                principalTable: "OrgaoExpedidor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Sexo_SexoId",
                table: "Pessoas",
                column: "SexoId",
                principalTable: "Sexo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Docentes_Pessoas_PessoaId",
                table: "Docentes");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoAgenda_Eventos_EventoId",
                table: "EventoAgenda");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoCota_Eventos_EventoId",
                table: "EventoCota");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Docentes_DocenteId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Eventos_EventoId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoHorario_Locais_LocalId",
                table: "EventoHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoRecurso_Eventos_EventoId",
                table: "EventoRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Pessoas_GEDTHPessoaId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_SolucoesEducacionais_CursoId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Entidades_EntidadeDemandanteId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Locais_LocalId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestores_Entidades_EntidadeId",
                table: "Gestores");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestores_Pessoas_PessoaId",
                table: "Gestores");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalContatos_Locais_LocalId",
                table: "LocalContatos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalRecursos_Locais_LocalId",
                table: "LocalRecursos");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalSalas_Locais_LocalId",
                table: "LocalSalas");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaContato_Pessoas_PessoaId",
                table: "PessoaContato");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Deficiencias_DeficienciaId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Entidades_EntidadeId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Escolaridades_EscolaridadeId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_OrgaoExpedidor_OrgaoExpedidorId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Sexo_SexoId",
                table: "Pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locais",
                table: "Locais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gestores",
                table: "Gestores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Escolaridades",
                table: "Escolaridades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Docentes",
                table: "Docentes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deficiencias",
                table: "Deficiencias");

            migrationBuilder.RenameTable(
                name: "Pessoas",
                newName: "Pessoa");

            migrationBuilder.RenameTable(
                name: "Locais",
                newName: "Local");

            migrationBuilder.RenameTable(
                name: "Gestores",
                newName: "Gestor");

            migrationBuilder.RenameTable(
                name: "Eventos",
                newName: "Evento");

            migrationBuilder.RenameTable(
                name: "Escolaridades",
                newName: "Escolaridade");

            migrationBuilder.RenameTable(
                name: "Docentes",
                newName: "Docente");

            migrationBuilder.RenameTable(
                name: "Deficiencias",
                newName: "Deficiencia");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_SexoId",
                table: "Pessoa",
                newName: "IX_Pessoa_SexoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_OrgaoExpedidorId",
                table: "Pessoa",
                newName: "IX_Pessoa_OrgaoExpedidorId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_EscolaridadeId",
                table: "Pessoa",
                newName: "IX_Pessoa_EscolaridadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_EntidadeId",
                table: "Pessoa",
                newName: "IX_Pessoa_EntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_DeficienciaId",
                table: "Pessoa",
                newName: "IX_Pessoa_DeficienciaId");

            migrationBuilder.RenameIndex(
                name: "IX_Gestores_PessoaId",
                table: "Gestor",
                newName: "IX_Gestor_PessoaId");

            migrationBuilder.RenameIndex(
                name: "IX_Gestores_EntidadeId",
                table: "Gestor",
                newName: "IX_Gestor_EntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_LocalId",
                table: "Evento",
                newName: "IX_Evento_LocalId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_EntidadeDemandanteId",
                table: "Evento",
                newName: "IX_Evento_EntidadeDemandanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_CursoId",
                table: "Evento",
                newName: "IX_Evento_CursoId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_GEDTHPessoaId",
                table: "Evento",
                newName: "IX_Evento_GEDTHPessoaId");

            migrationBuilder.RenameIndex(
                name: "IX_Docentes_PessoaId",
                table: "Docente",
                newName: "IX_Docente_PessoaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pessoa",
                table: "Pessoa",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Local",
                table: "Local",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gestor",
                table: "Gestor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evento",
                table: "Evento",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Escolaridade",
                table: "Escolaridade",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Docente",
                table: "Docente",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deficiencia",
                table: "Deficiencia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Docente_Pessoa_PessoaId",
                table: "Docente",
                column: "PessoaId",
                principalTable: "Pessoa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Pessoa_GEDTHPessoaId",
                table: "Evento",
                column: "GEDTHPessoaId",
                principalTable: "Pessoa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_SolucoesEducacionais_CursoId",
                table: "Evento",
                column: "CursoId",
                principalTable: "SolucoesEducacionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Entidades_EntidadeDemandanteId",
                table: "Evento",
                column: "EntidadeDemandanteId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Local_LocalId",
                table: "Evento",
                column: "LocalId",
                principalTable: "Local",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoAgenda_Evento_EventoId",
                table: "EventoAgenda",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoCota_Evento_EventoId",
                table: "EventoCota",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Docente_DocenteId",
                table: "EventoHorario",
                column: "DocenteId",
                principalTable: "Docente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Evento_EventoId",
                table: "EventoHorario",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoHorario_Local_LocalId",
                table: "EventoHorario",
                column: "LocalId",
                principalTable: "Local",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoRecurso_Evento_EventoId",
                table: "EventoRecurso",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestor_Entidades_EntidadeId",
                table: "Gestor",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestor_Pessoa_PessoaId",
                table: "Gestor",
                column: "PessoaId",
                principalTable: "Pessoa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalContatos_Local_LocalId",
                table: "LocalContatos",
                column: "LocalId",
                principalTable: "Local",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalRecursos_Local_LocalId",
                table: "LocalRecursos",
                column: "LocalId",
                principalTable: "Local",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalSalas_Local_LocalId",
                table: "LocalSalas",
                column: "LocalId",
                principalTable: "Local",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_Deficiencia_DeficienciaId",
                table: "Pessoa",
                column: "DeficienciaId",
                principalTable: "Deficiencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_Entidades_EntidadeId",
                table: "Pessoa",
                column: "EntidadeId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoa_Escolaridade_EscolaridadeId",
                table: "Pessoa",
                column: "EscolaridadeId",
                principalTable: "Escolaridade",
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
                name: "FK_PessoaContato_Pessoa_PessoaId",
                table: "PessoaContato",
                column: "PessoaId",
                principalTable: "Pessoa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
