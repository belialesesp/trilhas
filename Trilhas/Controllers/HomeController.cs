using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Trilhas.Data.Model.Eventos;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models.Trilhas.Eixo;
using Trilhas.Models.Trilhas.Estacao;
using Trilhas.Models.Usuario;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize]
    public class HomeController : DefaultController
    {
        private readonly TrilhasService _trilhaService;
        private readonly CadastroService _cadastroService;

        private readonly EixoService _eixoService;
        private readonly EstacaoService _estacaoService;
        private readonly SolucaoEducacionalService _solucaoEducacionalService;
        private readonly EventoService _eventoService;


        public HomeController(
            UserManager<IdentityUser> userManager,
            TrilhasService serviceTrilha,
            EixoService eixoService,
            EstacaoService estacaoService,
            SolucaoEducacionalService solucaoEducacionalService,
            EventoService eventoService,
            CadastroService serviceCadastro
            ) : base(userManager)
        {
            _estacaoService = estacaoService;
            _solucaoEducacionalService = solucaoEducacionalService;
            _eixoService = eixoService;
            _trilhaService = serviceTrilha;
            _cadastroService = serviceCadastro;
            _eventoService = eventoService;
        }

        public IActionResult Index()
        {
            return View(CarregarEixoEstacoes());
        }

        private List<EixoEstacoesViewModel> CarregarEixoEstacoes()
        {
            List<EixoEstacoesViewModel> viewModel = new List<EixoEstacoesViewModel>();
            List<Estacao> estacoes;
            List<Eixo> eixos = _eixoService.RecupararEixosComImagem();
            TrilhaDoUsuario trilha = _trilhaService.RecuperarTrilhaDoUsuario(RecuperarUsuarioId());

            foreach (var eixo in eixos)
            {
                estacoes = _estacaoService.RecupararEstacoesEixo(eixo.Id);
                viewModel.Add(new EixoEstacoesViewModel
                {
                    Eixo = new EixoViewModel(eixo.Id)
                    {
                        Nome = eixo.Nome,
                        Descricao = eixo.Descricao,
                        Imagem = eixo.Imagem,
                        Excluido = false
                    },
                    Estacoes = MapearEstacoes(trilha, estacoes)
                });
            }

            return viewModel;
        }

        private List<EstacaoViewModel> MapearEstacoes(TrilhaDoUsuario trilha, List<Estacao> estacoes)
        {
            List<EstacaoViewModel> retorno = new List<EstacaoViewModel>();

            foreach (var estacao in estacoes)
            {
                retorno.Add(new EstacaoViewModel(estacao.Id)
                {
                    Nome = estacao.Nome,
                    Descricao = estacao.Descricao,
                    EixoId = estacao.Eixo.Id,
                    Excluido = false,
                    Selecionado = trilha != null ? trilha.ContemEstacao(estacao) : false
                });
            }

            return retorno;
        }

        public IActionResult EscolhaSolucao(long estacaoId)
        {
            EstacaoSolucoesViewModel viewModel = (estacaoId != 0) ? CarregarEstacaoSolucoes(estacaoId) : null;
            return View(viewModel);
        }

        private EstacaoSolucoesViewModel CarregarEstacaoSolucoes(long estacaoId)
        {
            Estacao estacao = _estacaoService.RecuperarEstacao(estacaoId);

            if (estacao != null)
            {
                TrilhaDoUsuario trilha = _trilhaService.RecuperarTrilhaDoUsuario(RecuperarUsuarioId());
                List<SolucaoEducacional> solucoesEducacionais = _solucaoEducacionalService.RecuperarSolucoesEducacionais(estacao.Id);

                EstacaoSolucoesViewModel viewModel = new EstacaoSolucoesViewModel
                {
                    EixoNome = estacao.Eixo.Nome,
                    EixoImagem = _eixoService.RecuperarImagemEixo(estacao.Eixo.Id),
                    EstacaoNome = estacao.Nome,
                    EstacaoId = estacao.Id,
                    Solucoes = MapearSolucoes(trilha, solucoesEducacionais)
                };

                return viewModel;
            }
            else
            {
                return null;
            }
        }

        private List<SolucaoEducacionalOpcaoViewModel> MapearSolucoes(TrilhaDoUsuario trilha, List<SolucaoEducacional> solucoes)
        {
            List<SolucaoEducacionalOpcaoViewModel> retorno = new List<SolucaoEducacionalOpcaoViewModel>();
            SolucaoEducacionalOpcaoViewModel vm;

            foreach (var solucao in solucoes)
            {
                vm = new SolucaoEducacionalOpcaoViewModel(solucao.Id)
                {
                    Titulo = solucao.Titulo,
                    TipoDeSolucao = solucao.TipoDeSolucao,
                };

                if (solucao.TipoDeSolucao == "curso")
                {
                    Curso curso = (Curso)_solucaoEducacionalService.RecuperarSolucaoEducacionalCompleta(solucao.Id);
                    vm.ConteudoProgramatico = curso.ConteudoProgramatico;
                    vm.CargaHorariaTotal = curso.CargaHorariaTotal();
                    vm.PublicoAlvo = curso.PublicoAlvo;
                    vm.Modalidade = curso.Modalidade.ToString();

                    vm.Habilidades = new List<string>();

                    foreach (var habilidade in curso.Habilidades)
                    {
                        vm.Habilidades.Add(habilidade.Descricao);
                    }
                }
                else if (solucao.TipoDeSolucao == "livro")
                {
                    Livro livro = (Livro)_solucaoEducacionalService.RecuperarSolucaoEducacionalCompleta(solucao.Id);
                    vm.Autor = livro.Autor;
                    vm.DataPublicacao = livro.DataPublicacao.HasValue ? livro.DataPublicacao.Value.ToShortDateString() : "";
                    vm.Url = livro.Url;
                }
                else if (solucao.TipoDeSolucao == "video")
                {
                    Video video = (Video)_solucaoEducacionalService.RecuperarSolucaoEducacionalCompleta(solucao.Id);
                    vm.Responsavel = video.Responsavel;
                    vm.Duracao = video.Duracao;
                    vm.DataProducao = video.DataProducao.HasValue ? video.DataProducao.Value.ToShortDateString() : "";
                    vm.Url = video.Url;
                }

                vm.Selecionado = trilha != null ? trilha.ContemSolucao(solucao) : false;

                retorno.Add(vm);
            }

            return retorno;
        }

        public IActionResult VisualizarTrilha()
        {
            VisualizarTrilhaViewModel vm = new VisualizarTrilhaViewModel();

            var trilha = _trilhaService.RecuperarTrilhaCompletaDoUsuario(RecuperarUsuarioId());
			vm.UsuarioEmail = RecuperarUsuarioEmail();
			//adicionar metodo de Carregar CPF do usuario logado
			vm.UsuarioCPF = RecuperarUsuarioId();
			if(trilha == null)
            {
                return View(vm);
            }

            foreach (var item in trilha.ItensAtivos())
            {
                if (!item.SolucaoEducacional.DeletionTime.HasValue &&
                    !item.SolucaoEducacional.Estacao.DeletionTime.HasValue &&
                    !item.SolucaoEducacional.Estacao.Eixo.DeletionTime.HasValue)
                {
                    vm.Adicionar(item.SolucaoEducacional.Estacao.Eixo.Nome,
                        item.SolucaoEducacional.Estacao.Nome,
                        item.SolucaoEducacional.Id,
                        item.SolucaoEducacional.Titulo,
                        item.SolucaoEducacional.TipoDeSolucao);
                }
            }

            return View(vm);
        }

        public IActionResult AdicionarTrilha(long solucaoId)
        {
            var usuario = RecuperarUsuarioId();

            if (!string.IsNullOrEmpty(usuario))
            {
                var solucao = _solucaoEducacionalService.RecuperarSolucaoEducacional(solucaoId);

                if (solucao != null)
                {
                    _trilhaService.AdicionarTrilha(usuario, solucao);

                    return RedirectToAction("EscolhaSolucao", new { estacaoId = solucao.Estacao.Id });
                }
            }

            return View("Index");
        }

        public IActionResult RemoverDaTrilha(long solucaoId)
        {
            var usuario = RecuperarUsuarioId();

            if (!string.IsNullOrEmpty(usuario))
            {
                _trilhaService.RemoverDaTrilha(usuario, solucaoId);
            }

            return RedirectToAction("VisualizarTrilha");
        }

        public IActionResult VisualizarCertificados()
        {
            VisualizarCertificadosViewModel vm = new VisualizarCertificadosViewModel();

            var usuario = long.Parse(RecuperarUsuarioId());

            List<Inscrito> lista = _eventoService.RecuperaEventoInscricao(1);
            vm.IdUsuario = usuario;
            vm.UsuarioEmail = RecuperarUsuarioEmail();
			
			//adicionar metodo de Carregar CPF do usuario logado
			vm.UsuarioCPF = RecuperarUsuarioId();

			foreach(var item in lista)
            {
                vm.EventosFinalizados.Add(new EventoFinalizadoViewModel
                {
                    CursistaId = item.Id,
                    Eixo = item.ListaDeInscricao.Evento.Curso.Estacao.Eixo.Nome,
                    Estacao = item.ListaDeInscricao.Evento.Curso.Estacao.Nome,
                    Solucao = item.ListaDeInscricao.Evento.Curso.Titulo,
                    Situacao = item.Situacao.ToString()
				});
            }

            return View(vm);
        }
    }
}
