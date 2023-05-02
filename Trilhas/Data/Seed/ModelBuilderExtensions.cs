using Microsoft.EntityFrameworkCore;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Data.Seed
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoDeCurso>().HasData(
                new TipoDeCurso { Id = 5, Descricao = "FÓRUM" },
                new TipoDeCurso { Id = 7, Descricao = "MESA REDONDA" },
                new TipoDeCurso { Id = 9, Descricao = "PALESTRA" },
                new TipoDeCurso { Id = 11, Descricao = "SEMINÁRIO" },
                new TipoDeCurso { Id = 13, Descricao = "WORKSHOP" }
            );

            modelBuilder.Entity<NivelDeCurso>().HasData(
                new NivelDeCurso { Id = 1, Descricao = "CAPACITAÇÃO" },
                new NivelDeCurso { Id = 2, Descricao = "FORMAÇÃO" }
            );

            modelBuilder.Entity<Municipio>().HasData(
                new Municipio { Id = 1, codigoMunicipio = 3205309, codigoUf = 32, NomeMunicipio = "VITÓRIA", Uf = "ES" },
                new Municipio { Id = 2, codigoMunicipio = 3205200, codigoUf = 32, NomeMunicipio = "VILA VELHA", Uf = "ES" },
                new Municipio { Id = 3, codigoMunicipio = 3205002, codigoUf = 32, NomeMunicipio = "SERRA", Uf = "ES" },
                new Municipio { Id = 4, codigoMunicipio = 3201308, codigoUf = 32, NomeMunicipio = "CARIACICA", Uf = "ES" }
            );

            modelBuilder.Entity<TipoDeEntidade>().HasData(
                new TipoDeEntidade { Id = 1, Descricao = "MUNICIPAL" },
                new TipoDeEntidade { Id = 2, Descricao = "ESTADUAL" },
                new TipoDeEntidade { Id = 3, Descricao = "FEDERAL" },
                new TipoDeEntidade { Id = 4, Descricao = "ORGANIZAÇÃO NÃO GOVERNAMENTAL" }
            );

            //modelBuilder.Entity<Entidade>().HasData(
            //    new Entidade
            //    {
            //        Id = 1,
            //        Sigla = "ESESP",
            //        Nome = "ESCOLA DE SERVIÇO PÚBLICO DO ESPÍRITO SANTO",
            //        TipoId = 2, 
            //        CreationTime = DateTime.Now
            //    }
            //);

            modelBuilder.Entity<Deficiencia>().HasData(
                new Deficiencia { Id = 1, Nome = "FÍSICA" },
                new Deficiencia { Id = 2, Nome = "AUDITIVA" },
                new Deficiencia { Id = 3, Nome = "VISUAL" },
                new Deficiencia { Id = 4, Nome = "MENTAL" }
            );

            modelBuilder.Entity<Escolaridade>().HasData(
                new Escolaridade { Id = 1, Nome = "ALFABETIZADO" },
                new Escolaridade { Id = 2, Nome = "ANALFABETO" },
                new Escolaridade { Id = 3, Nome = "DOUTOR" },
                new Escolaridade { Id = 4, Nome = "MESTRE" },
                new Escolaridade { Id = 5, Nome = "PÓS GRADUADO" },
                new Escolaridade { Id = 6, Nome = "PRIMEIRO GRAU COMPLETO" },
                new Escolaridade { Id = 7, Nome = "PRIMEIRO GRAU INCOMPLETO" },
                new Escolaridade { Id = 8, Nome = "SEGUNDO GRAU COMPLETO" },
                new Escolaridade { Id = 9, Nome = "SEGUNDO GRAU INCOMPLETO" },
                new Escolaridade { Id = 10, Nome = "SUPERIOR COMPLETO" },
                new Escolaridade { Id = 11, Nome = "SUPERIOR INCOMPLETO" }
            );

            modelBuilder.Entity<FuncaoDocente>().HasData(
                new FuncaoDocente { Id = 1, Descricao = "DOCENTE" },
                new FuncaoDocente { Id = 2, Descricao = "DOCENTE ASSISTENTE" },
                new FuncaoDocente { Id = 3, Descricao = "CONFERENCISTA" },
                new FuncaoDocente { Id = 4, Descricao = "PALESTRANTE" },
                new FuncaoDocente { Id = 5, Descricao = "PAINELISTA" },
                new FuncaoDocente { Id = 6, Descricao = "DEBATEDOR" },
                new FuncaoDocente { Id = 7, Descricao = "MODERADOR" }
            );

            modelBuilder.Entity<OrgaoExpedidor>().HasData(
                new OrgaoExpedidor { Id = 1, Nome = "SECRETARIA DE SEGURANÇA PÚBLICA", Sigla = "SSP" }
            );

            modelBuilder.Entity<Sexo>().HasData(
                new Sexo { Id = 1, Nome = "MASCULINO" },
                new Sexo { Id = 2, Nome = "FEMININO" },
                new Sexo { Id = 3, Nome = "OUTRO" }
            );

            modelBuilder.Entity<TipoLocalContato>().HasData(
                new TipoLocalContato { Id = 1, Nome = "RESIDENCIAL" },
                new TipoLocalContato { Id = 2, Nome = "COMERCIAL" },
                new TipoLocalContato { Id = 3, Nome = "CELULAR" },
                new TipoLocalContato { Id = 4, Nome = "OUTROS" }
            );

            modelBuilder.Entity<TipoPessoaContato>().HasData(
                new TipoPessoaContato { Id = 1, Nome = "RESIDENCIAL" },
                new TipoPessoaContato { Id = 2, Nome = "COMERCIAL" },
                new TipoPessoaContato { Id = 3, Nome = "CELULAR" }
            );
        }
    }
}
