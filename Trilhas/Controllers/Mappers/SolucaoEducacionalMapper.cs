using System.Collections.Generic;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Trilhas.SolucaoEducacional;

namespace Trilhas.Controllers.Mappers
{
    public class SolucaoEducacionalMapper
    {
        public SolucaoEducacionalViewModel MapearSolucaoEducacionalBasicoViewModel(SolucaoEducacional solucao)
        {
            SolucaoEducacionalViewModel vm = new SolucaoEducacionalViewModel(solucao.Id);
            vm.EstacaoId = solucao.Estacao.Id;
            vm.TipoDeSolucao = solucao.TipoDeSolucao;
            vm.Titulo = solucao.TipoDeSolucao.Equals("curso") ? ((Curso)solucao).Sigla + " - " + solucao.Titulo : solucao.Titulo;

            return vm;
        }

        public SolucaoEducacionalViewModel MapearSolucaoEducacionalViewModel(SolucaoEducacional solucao)
        {
            SolucaoEducacionalViewModel vm = new SolucaoEducacionalViewModel(solucao.Id);
            vm.EstacaoId = solucao.Estacao.Id;
            vm.TipoDeSolucao = solucao.TipoDeSolucao;
            vm.Titulo = solucao.Titulo;

            switch (solucao.TipoDeSolucao)
            {
                case "curso":
                    Curso curso = (Curso)solucao;
                    vm.Modalidade = curso.Modalidade;
                    vm.TipoCursoId = curso.TipoDoCurso.Id;
                    vm.NivelCursoId = curso.NivelDoCurso.Id;
                    vm.Sigla = curso.Sigla;
                    vm.Descricao = curso.Descricao;
                    vm.FrequenciaMinimaCertificado = curso.FrequenciaMinimaCertificado;
                    vm.FrequenciaMinimaDeclaracao = curso.FrequenciaMinimaDeclaracao;
                    vm.PermiteCertificado = curso.PermiteCertificado;
                    vm.PreRequisitos = curso.PreRequisitos;
                    vm.PublicoAlvo = curso.PublicoAlvo;
                    vm.ConteudoProgramatico = curso.ConteudoProgramatico;
                    vm.CargaHorariaTotal = curso.CargaHorariaTotal();

                    vm.Habilidades = new List<HabilidadeViewModel>();
                    foreach (var habilidade in curso.Habilidades)
                    {
                        vm.Habilidades.Add(new HabilidadeViewModel
                        {
                            Id = habilidade.Id,
                            Descricao = habilidade.Descricao
                        });
                    }

                    vm.Modulos = new List<ModuloViewModel>();
                    foreach (var modulo in curso.Modulos)
                    {
                        vm.Modulos.Add(new ModuloViewModel
                        {
                            Id = modulo.Id,
                            Nome = modulo.Nome,
                            Descricao = modulo.Descricao,
                            CargaHoraria = modulo.CargaHoraria
                        });
                    }
                    break;
                case "livro":
                    Livro livro = (Livro)solucao;
                    vm.Autor = livro.Autor;
                    vm.Url = livro.Url;
                    vm.Editora = livro.Editora;
                    vm.DataPublicacao = livro.DataPublicacao.HasValue ? livro.DataPublicacao.Value.ToString("yyyy-MM-dd") : "";
                    vm.Edicao = livro.Edicao;
                    vm.OutrasInformacoes = livro.OutrasInformacoes;
                    break;
                case "video":
                    Video video = (Video)solucao;
                    vm.Responsavel = video.Responsavel;
                    vm.Url = video.Url;
                    vm.DataProducao = video.DataProducao.HasValue ? video.DataProducao.Value.ToString("yyyy-MM-dd") : "";
                    vm.Duracao = video.Duracao;
                    vm.OutrasInformacoes = video.OutrasInformacoes;
                    break;
            }

            return vm;
        }

        public List<GridSolucaoEducacionalViewModel> MapearSolucoesEducacionaisViewModel(List<SolucaoEducacional> solucoesEducacionais)
        {
            List<GridSolucaoEducacionalViewModel> gridSolucoesVm = new List<GridSolucaoEducacionalViewModel>();

            foreach (var solucao in solucoesEducacionais)
            {
                var vm = new GridSolucaoEducacionalViewModel
                {
                    Id = solucao.Id,
                    EixoNome = solucao.Estacao.Eixo.Nome,
                    EstacaoNome = solucao.Estacao.Nome,
                    Titulo = solucao.Titulo,
                    Excluido = solucao.DeletionTime.HasValue
                };

                switch (solucao.TipoDeSolucao)
                {
                    case "curso":
                        Curso curso = (Curso)solucao;
                        vm.TipoDeSolucao = "Curso";
                        vm.ModalidadeDeCurso.Id = (int)curso.Modalidade;
                        vm.ModalidadeDeCurso.Descricao = curso.Modalidade.ToString();
                        vm.Titulo = curso.Sigla + " - " + curso.Titulo;
                        vm.PermiteCertificado = curso.PermiteCertificado;

                        if (curso.Modulos != null)
                        {
                            vm.CargaHorariaTotal = curso.CargaHorariaTotal();

                            foreach (var modulo in curso.Modulos)
                            {
                                vm.Modulos.Add(new ModuloViewModel
                                {
                                    Id = modulo.Id,
                                    Nome = modulo.Nome,
                                    Descricao = modulo.Descricao,
                                    CargaHoraria = modulo.CargaHoraria
                                });
                            }
                        }
                        break;
                    case "livro": vm.TipoDeSolucao = "Livro/Artivo"; break;
                    case "video": vm.TipoDeSolucao = "Vídeo"; break;
                }

                gridSolucoesVm.Add(vm);
            }

            return gridSolucoesVm;
        }
    }
}
