using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Trilhas.SolucaoEducacional;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class SolucoesController : DefaultController
    {
        private readonly EstacaoService _estacaoService;
        private readonly SolucaoEducacionalService _solucaoEducacionalService;
        private readonly SolucaoEducacionalMapper _mapper;

        public SolucoesController(
            UserManager<IdentityUser> userManager,
            TrilhasService trilhaService,
            EstacaoService estacaoService,
            SolucaoEducacionalService solucaoEducacionalService) : base(userManager)
        {
            _solucaoEducacionalService = solucaoEducacionalService;
            _estacaoService = estacaoService;
            _mapper = new SolucaoEducacionalMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(long eixoId, long estacaoId, string titulo, string tipoSolucao, string autor, string editora, string responsavel, string sigla, EnumModalidade? modalidadeCurso, long nivelCurso, bool excluidos)
        {
            int qtd = _solucaoEducacionalService.QuantidadeDeSolucoesEducacionais(modalidadeCurso, eixoId, estacaoId, titulo, tipoSolucao, autor, editora, responsavel, sigla, nivelCurso, excluidos);

            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(long eixoId, long estacaoId, string titulo, string tipoSolucao, string autor, string editora, string responsavel, string sigla, EnumModalidade? modalidadeCurso, long nivelCurso, bool excluidos, int start, int count)
        {
            List<SolucaoEducacional> solucoesEducacionais = _solucaoEducacionalService.PesquisarSolucoesEducacionais(modalidadeCurso, eixoId, estacaoId, titulo, tipoSolucao, autor, editora, responsavel, sigla, nivelCurso, excluidos, start, count);

            var vm = _mapper.MapearSolucoesEducacionaisViewModel(solucoesEducacionais);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            SolucaoEducacional solucoesEducacionais = _solucaoEducacionalService.RecuperarSolucaoEducacionalCompleta(id);

            var vm = _mapper.MapearSolucaoEducacionalViewModel(solucoesEducacionais);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarBasico(long id)
        {
            SolucaoEducacional solucoesEducacionais = _solucaoEducacionalService.RecuperarSolucaoEducacional(id);

            var vm = _mapper.MapearSolucaoEducacionalBasicoViewModel(solucoesEducacionais);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _solucaoEducacionalService.ExcluirSolucaoEducacional(RecuperarUsuarioId(), id);
            }
            catch (RecordNotFoundException rex)
            {
                return BadRequest(rex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public IActionResult SalvarCurso([FromBody] SalvarCursoViewModel vm)
        {
            try
            {
                ValidarCurso(vm);

                SolucaoEducacional solucao;

                if (vm.Id > 0)
                {
                    solucao = AtualizarSolucaoCurso(vm);
                }
                else
                {
                    solucao = CriarSolucaoCurso(vm);
                }

                solucao = _solucaoEducacionalService.SalvarSolucaoEducacional(RecuperarUsuarioId(), solucao);

                return JsonFormResponse(solucao.Id);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao salvar o registro.");
            }
        }

        private SolucaoEducacional CriarSolucaoCurso(SalvarCursoViewModel vm)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);

            TipoDeCurso tipo = _solucaoEducacionalService.RecuperarTipoDeCurso(vm.TipoCursoId);
            NivelDeCurso nivel = _solucaoEducacionalService.RecuperarNivelDeCurso(vm.NivelCursoId);

            Curso curso = new Curso
            {
                Estacao = estacao,
                Modalidade = vm.Modalidade,
                TipoDoCurso = tipo,
                NivelDoCurso = nivel,
                Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null,
                Sigla = !string.IsNullOrEmpty(vm.Sigla) ? vm.Sigla.Trim() : null,
                Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null,
                PreRequisitos = !string.IsNullOrEmpty(vm.PreRequisitos) ? vm.PreRequisitos.Trim() : null,
                PublicoAlvo = !string.IsNullOrEmpty(vm.PublicoAlvo) ? vm.PublicoAlvo.Trim() : null,
                ConteudoProgramatico = !string.IsNullOrEmpty(vm.ConteudoProgramatico) ? vm.ConteudoProgramatico.Trim() : null,
                PermiteCertificado = vm.PermiteCertificado,
                FrequenciaMinimaCertificado = vm.FrequenciaMinimaCertificado,
                FrequenciaMinimaDeclaracao = vm.FrequenciaMinimaDeclaracao,
                Habilidades = new List<Habilidade>()
            };

            if (vm.Habilidades != null)
            {
                foreach (var habilidadeVm in vm.Habilidades)
                {
                    curso.Habilidades.Add(new Habilidade()
                    {
                        Id = habilidadeVm.Id,
                        Curso = curso,
                        Descricao = habilidadeVm.Descricao
                    });
                }
            }

            if (vm.Modulos != null)
            {
                foreach (var moduloVm in vm.Modulos)
                {
                    curso.Modulos.Add(new Modulo()
                    {
                        Id = moduloVm.Id,
                        Curso = curso,
                        Nome = moduloVm.Nome,
                        Descricao = moduloVm.Descricao,
                        CargaHoraria = moduloVm.CargaHoraria
                    });
                }
            }

            return curso;
        }

        private SolucaoEducacional AtualizarSolucaoCurso(SalvarCursoViewModel vm)
        {
            SolucaoEducacional solucao = _solucaoEducacionalService.RecuperarSolucaoEducacionalCompleta(vm.Id);

            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);
            TipoDeCurso tipo = _solucaoEducacionalService.RecuperarTipoDeCurso(vm.TipoCursoId);
            NivelDeCurso nivel = _solucaoEducacionalService.RecuperarNivelDeCurso(vm.NivelCursoId);

            Curso curso = (Curso)solucao;

            curso.Estacao = estacao;
            curso.Modalidade = vm.Modalidade;
            curso.TipoDoCurso = tipo;
            curso.NivelDoCurso = nivel;
            curso.PermiteCertificado = vm.PermiteCertificado;
            curso.Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null;
            curso.Descricao = !string.IsNullOrEmpty(vm.Descricao) ? vm.Descricao.Trim() : null;
            curso.Sigla = !string.IsNullOrEmpty(vm.Sigla) ? vm.Sigla.Trim() : null;
            curso.FrequenciaMinimaCertificado = vm.FrequenciaMinimaCertificado;
            curso.FrequenciaMinimaDeclaracao = vm.FrequenciaMinimaDeclaracao;
            curso.PublicoAlvo = !string.IsNullOrEmpty(vm.PublicoAlvo) ? vm.PublicoAlvo.Trim() : null;
            curso.ConteudoProgramatico = !string.IsNullOrEmpty(vm.ConteudoProgramatico) ? vm.ConteudoProgramatico.Trim() : null;

            curso.Habilidades = new List<Habilidade>();

            if (vm.Habilidades != null)
            {
                foreach (var habilidadeVm in vm.Habilidades.Take(5))
                {
                    curso.Habilidades.Add(new Habilidade()
                    {
                        Id = habilidadeVm.Id,
                        Curso = curso,
                        Descricao = habilidadeVm.Descricao
                    });
                }
            }

            if (vm.Modulos != null)
            {
                foreach (var moduloVm in vm.Modulos)
                {
                    curso.Modulos.Add(new Modulo()
                    {
                        Id = moduloVm.Id,
                        Curso = curso,
                        Nome = moduloVm.Nome,
                        Descricao = moduloVm.Descricao,
                        CargaHoraria = moduloVm.CargaHoraria
                    });
                }
            }

            return solucao;
        }

        [HttpPost]
        public IActionResult SalvarLivro([FromBody] SalvarLivroViewModel vm)
        {
            try
            {
                ValidarLivro(vm);

                SolucaoEducacional solucao;

                if (vm.Id > 0)
                {
                    solucao = AtualizarSolucaoLivro(vm);
                }
                else
                {
                    solucao = CriarSolucaoLivro(vm);
                }

                solucao = _solucaoEducacionalService.SalvarSolucaoEducacional(RecuperarUsuarioId(), solucao);

                return JsonFormResponse(solucao.Id);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex);
            }
        }

        private SolucaoEducacional CriarSolucaoLivro(SalvarLivroViewModel vm)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);

            Livro livro = new Livro
            {
                Estacao = estacao,
                Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null,
                Autor = !string.IsNullOrEmpty(vm.Autor) ? vm.Autor.Trim() : null,
                Url = !string.IsNullOrEmpty(vm.Url) ? vm.Url.Trim() : null,
                Editora = !string.IsNullOrEmpty(vm.Editora) ? vm.Editora.Trim() : null,
                DataPublicacao = vm.DataPublicacao,
                Edicao = !string.IsNullOrEmpty(vm.Edicao) ? vm.Edicao.Trim() : null,
                OutrasInformacoes = !string.IsNullOrEmpty(vm.OutrasInformacoes) ? vm.OutrasInformacoes.Trim() : null
            };

            return livro;
        }

        private SolucaoEducacional AtualizarSolucaoLivro(SalvarLivroViewModel vm)
        {
            SolucaoEducacional solucao = _solucaoEducacionalService.RecuperarSolucaoEducacional(vm.Id);

            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);

            Livro livro = (Livro)solucao;
            livro.Estacao = estacao;
            livro.Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null;
            livro.Autor = !string.IsNullOrEmpty(vm.Autor) ? vm.Autor.Trim() : null;
            livro.Url = !string.IsNullOrEmpty(vm.Url) ? vm.Url.Trim() : null;
            livro.Editora = !string.IsNullOrEmpty(vm.Editora) ? vm.Editora.Trim() : null;
            livro.DataPublicacao = vm.DataPublicacao;
            livro.Edicao = !string.IsNullOrEmpty(vm.Edicao) ? vm.Edicao.Trim() : null;
            livro.OutrasInformacoes = !string.IsNullOrEmpty(vm.OutrasInformacoes) ? vm.OutrasInformacoes.Trim() : null;

            return solucao;
        }

        [HttpPost]
        public IActionResult SalvarVideo([FromBody] SalvarVideoViewModel vm)
        {
            try
            {
                ValidarVideo(vm);

                SolucaoEducacional solucao;

                if (vm.Id > 0)
                {
                    solucao = AtualizarSolucaoVideo(vm);
                }
                else
                {
                    solucao = CriarSolucaoVideo(vm);
                }

                solucao = _solucaoEducacionalService.SalvarSolucaoEducacional(RecuperarUsuarioId(), solucao);

                return JsonFormResponse(solucao.Id);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex);
            }
        }

        private SolucaoEducacional CriarSolucaoVideo(SalvarVideoViewModel vm)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);

            Video video = new Video();
            video.Estacao = estacao;
            video.Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null;
            video.Responsavel = !string.IsNullOrEmpty(vm.Responsavel) ? vm.Responsavel.Trim() : null;
            video.Url = !string.IsNullOrEmpty(vm.Url) ? vm.Url.Trim() : null;
            video.DataProducao = vm.DataProducao;
            video.Duracao = !string.IsNullOrEmpty(vm.Duracao) ? vm.Duracao.Trim() : null;
            video.OutrasInformacoes = !string.IsNullOrEmpty(vm.OutrasInformacoes) ? vm.OutrasInformacoes.Trim() : null;

            return video;
        }

        private SolucaoEducacional AtualizarSolucaoVideo(SalvarVideoViewModel vm)
        {
            SolucaoEducacional solucao = _solucaoEducacionalService.RecuperarSolucaoEducacional(vm.Id);

            Estacao estacao = _estacaoService.RecuperarEstacao(vm.EstacaoId);

            Video video = (Video)solucao;
            video.Estacao = estacao;
            video.Titulo = !string.IsNullOrEmpty(vm.Titulo) ? vm.Titulo.Trim() : null;
            video.Responsavel = !string.IsNullOrEmpty(vm.Responsavel) ? vm.Responsavel.Trim() : null;
            video.Url = !string.IsNullOrEmpty(vm.Url) ? vm.Url.Trim() : null;
            video.DataProducao = vm.DataProducao;
            video.Duracao = !string.IsNullOrEmpty(vm.Duracao) ? vm.Duracao.Trim() : null;
            video.OutrasInformacoes = !string.IsNullOrEmpty(vm.OutrasInformacoes) ? vm.OutrasInformacoes.Trim() : null;

            return solucao;
        }


        private void ValidarCurso(SalvarCursoViewModel vm)
        {
            if (vm.EstacaoId <= 0)
            {
                ModelState.AddModelError("Estação", "Selecione a Estação.");
            }
            if (vm.TipoCursoId <= 0)
            {
                ModelState.AddModelError("Tipo Curso", "Preencha a Tipo Curso.");
            }
            //if (vm.ModalidadeId <= 0)
            //{
            //    ModelState.AddModelError("Modalidade", "Preencha a Modalidade");
            //}
            if (vm.NivelCursoId <= 0)
            {
                ModelState.AddModelError("Nível Curso", "Preencha a Nível Curso.");
            }
            if (string.IsNullOrWhiteSpace(vm.Titulo))
            {
                ModelState.AddModelError("Título", "Preencha o Título.");
            }
            if (string.IsNullOrWhiteSpace(vm.Sigla))
            {
                ModelState.AddModelError("Sigla", "Preencha a Sigla.");
            }
            if (string.IsNullOrWhiteSpace(vm.Descricao))
            {
                ModelState.AddModelError("Descrição", "Preencha a Descrição.");
            }
            if (vm.FrequenciaMinimaCertificado < 0 || vm.FrequenciaMinimaCertificado > 100)
            {
                ModelState.AddModelError("Frequência Mínima Certificado", "Preencha um valor entre 0 e 100 para Frequência Mínima para Certificado.");
            }
            if (vm.FrequenciaMinimaDeclaracao < 0 || vm.FrequenciaMinimaDeclaracao > 100)
            {
                ModelState.AddModelError("Frequência Mínima Declaração", "Preencha um valor entre 0 e 100 para Frequência Mínima para Declaração.");
            }
            if (string.IsNullOrWhiteSpace(vm.PublicoAlvo))
            {
                ModelState.AddModelError("Público Alvo", "Informe o Público Alvo da Solução.");
            }
            if (string.IsNullOrWhiteSpace(vm.ConteudoProgramatico))
            {
                ModelState.AddModelError("Conteúdo Programático", "Informe o Conteúdo Programático da Solução.");
            }
            if (vm.Habilidades == null || vm.Habilidades.Count < 1)
            {
                ModelState.AddModelError("Habilidades", "Informe no mínimo uma habilidade para Solução.");
            }
            if (vm.Modulos == null || vm.Modulos.Count < 1)
            {
                ModelState.AddModelError("Módulos", "Informe no mínimo um Módulo para Solução.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        private void ValidarVideo(SalvarVideoViewModel vm)
        {
            if (vm.EstacaoId <= 0)
            {
                ModelState.AddModelError("Estação", "Selecione a Estação.");
            }
            if (string.IsNullOrWhiteSpace(vm.Responsavel.Trim()))
            {
                ModelState.AddModelError("Responsável", "Preencha o campo Responsável.");
            }
            if (string.IsNullOrWhiteSpace(vm.Titulo.Trim()))
            {
                ModelState.AddModelError("Título", "Preencha o campo Título.");
            }
            if (string.IsNullOrWhiteSpace(vm.Duracao.Trim()))
            {
                ModelState.AddModelError("Duração", "Preencha o campo Duração.");
            }
            if (string.IsNullOrWhiteSpace(vm.Url.Trim()))
            {
                ModelState.AddModelError("Url", "Preencha o campo Url.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        private void ValidarLivro(SalvarLivroViewModel vm)
        {
            if (vm.EstacaoId <= 0)
            {
                ModelState.AddModelError("Estação", "Selecione a Estação.");
            }
            if (string.IsNullOrWhiteSpace(vm.Autor))
            {
                ModelState.AddModelError("Autor", "Preencha o campo Autor.");
            }
            if (string.IsNullOrWhiteSpace(vm.Titulo))
            {
                ModelState.AddModelError("Título", "Preencha o campo Título.");
            }
            if (string.IsNullOrWhiteSpace(vm.Editora))
            {
                ModelState.AddModelError("Editora", "Preencha o campo Editora.");
            }
            if (string.IsNullOrWhiteSpace(vm.Edicao))
            {
                ModelState.AddModelError("Edição", "Preencha o campo Edição.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        [HttpGet]
        public IActionResult BuscarTiposDeCurso()
        {
            try
            {
                List<TipoDeCurso> tipos = _solucaoEducacionalService.RecuperarTiposDeCurso();
                return JsonFormResponse(tipos);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex);
            }
        }

        [HttpGet]
        public IActionResult BuscarNiveisDeCurso()
        {
            try
            {
                List<NivelDeCurso> niveis = _solucaoEducacionalService.RecuperarNiveisDeCurso();
                return JsonFormResponse(niveis);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex);
            }
        }
    }
}
