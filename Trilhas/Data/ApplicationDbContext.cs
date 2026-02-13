using Microsoft.EntityFrameworkCore;
using System.Linq;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Data.Seed;
using Trilhas.SqlDto;
using Trilhas.Data.Model.TermosReferencia;
using Trilhas.Data.Model.Notifications;

namespace Trilhas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolucaoEducacional>()
            .HasDiscriminator<string>("TipoDeSolucao")
            .HasValue<Curso>("curso")
            .HasValue<Video>("video")
            .HasValue<Livro>("livro");

            // Configure TermoReferenciaItem decimal precision to fix the warning
            modelBuilder.Entity<TermoReferenciaItem>()
                .Property(t => t.CargaHoraria)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TermoReferenciaItem>()
                .Property(t => t.ValorHora)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TermoReferenciaItem>()
                .Property(t => t.EncargosPercentual)
                .HasPrecision(5, 2);

            modelBuilder.Entity<TermoReferenciaItem>()
                .Property(t => t.ValorTotal)
                .HasPrecision(18, 2);

            modelBuilder.Seed();
        }

        public bool IsAttached<T>(T entity) where T : class
        {
            return this.Set<T>().Local.Any(e => e == entity);
        }

        public DbSet<Inscrito> Inscritos { get; set; }
        public DbSet<ListaDeInscricao> ListaDeInscricao { get; set; }

        public DbSet<Habilitacao> Habilitacao { get; set; }
        public DbSet<Formacao> Formacao { get; set; }
        public DbSet<DadosBancarios> DadosBancarios { get; set; }


        public DbSet<Docente> Docentes { get; set; }
        public DbSet<FuncaoDocente> FuncoesDocente { get; set; }
        public DbSet<EventoHorario> EventoHorario { get; set; }
        public DbSet<EventoRecurso> EventoRecurso { get; set; }
        public DbSet<EventoCota> EventoCota { get; set; }
        public DbSet<EventoAgenda> EventoAgenda { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Penalidade> Penalidades { get; set; }
        public DbSet<Sexo> Sexo { get; set; }
        public DbSet<OrgaoExpedidor> OrgaoExpedidor { get; set; }
        public DbSet<Deficiencia> Deficiencias { get; set; }
        public DbSet<Escolaridade> Escolaridades { get; set; }
        public DbSet<TipoPessoaContato> TipoPessoaContato { get; set; }
        public DbSet<PessoaContato> PessoaContato { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Gestor> Gestores { get; set; }

        public DbSet<Local> Locais { get; set; }
        public DbSet<LocalContato> LocalContatos { get; set; }
        public DbSet<LocalRecurso> LocalRecursos { get; set; }
        public DbSet<LocalSala> LocalSalas { get; set; }
        public DbSet<TipoLocalContato> TipoLocalContato { get; set; }

        public DbSet<Eixo> Eixos { get; set; }
        public DbSet<Estacao> Estacoes { get; set; }
        public DbSet<SolucaoEducacional> SolucoesEducacionais { get; set; }
        public DbSet<Habilidade> Habilidades { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<TipoDeCurso> TiposDeCurso { get; set; }
        public DbSet<NivelDeCurso> NiveisDeCurso { get; set; }
        public DbSet<Entidade> Entidades { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<TipoDeEntidade> TiposDeEntidade { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<TrilhaDoUsuario> Trilhas { get; set; }
        public DbSet<ItemDaTrilha> ItensDaTrilha { get; set; }
        public DbSet<RegistroDePresenca> RegistrosDePresenca { get; set; }
        public DbSet<Certificado> Certificados { get; set; }
        public DbSet<CertificadoEmitido> CertificadosEmitidos { get; set; }
        public DbSet<TermoDeReferencia> TermosDeReferencia { get; set; }
        public DbSet<TermoReferenciaItem> TermoReferenciaItens { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        #region Dto retorno query sql
        public DbSet<GridDocenteDto> GridDocenteDto { get; set; }
        public DbSet<GridCursistaDto> GridCursistaDto { get; set; }
		#endregion
	}
}