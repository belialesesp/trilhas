using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Certificado;
using Trilhas.Models.Relatorio;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class RelatoriosController : Controller
    {
		private readonly EventoService _eventoService;
		private readonly PessoaService _pessoaService;
		private readonly CertificadoService _certificadoService;
		private readonly CertificadoMapper _mapper;
        private readonly RelatorioService _relatorioService;

        public RelatoriosController(PessoaService pessoaService, EventoService eventoService, CertificadoService certificadoService, RelatorioService relatorioService)
		{
			_pessoaService = pessoaService;
			_eventoService = eventoService;
			_mapper = new CertificadoMapper();
			_certificadoService = certificadoService;
			_relatorioService = relatorioService;

        }

		public IActionResult Certificado(long inscricaoId)
		{
			var inscricao = _eventoService.RecuperarInscricao(inscricaoId);

			if(inscricao == null)
			{
				return this.BadRequest();
			}

			EmissaoCertificadoViewModel vm = _mapper.MapearEmissaoCertificado(inscricao, "", inscricao.ListaDeInscricao.Evento.Certificado.Dados);

			return View(vm);
		}
		public IActionResult RelatorioCursista(long eventoId)
		{
			RelatorioCursistaViewModel vm = new RelatorioCursistaViewModel();

			Evento evento = _eventoService.RecuperarEventoListaPresenca(eventoId);
			vm = _mapper.MapearRelatorioCursista(evento);

			return View(vm);
		}

		public IActionResult RelatorioHistoricoCursista(long cursistaId)
		{
			RelatorioHistoricoCursistaViewModel vm = new RelatorioHistoricoCursistaViewModel();

			Pessoa cursista = _pessoaService.RecuperarPessoaCompleto(cursistaId);

			List<Evento> eventos = _eventoService.RecuperaEventoCursista(cursistaId);
			vm = _mapper.MapearRelatorioHistoricoCursista(cursista, eventos);
						
			return View(vm);
		}

        [HttpGet]
        public IActionResult ExportarRelatorioHistoricoCursistaExcel(long cursistaId)
        {
            RelatorioHistoricoCursistaViewModel vm = new RelatorioHistoricoCursistaViewModel();

            Pessoa cursista = _pessoaService.RecuperarPessoaCompleto(cursistaId);

            List<Evento> eventos = _eventoService.RecuperaEventoCursista(cursistaId);
            vm = _mapper.MapearRelatorioHistoricoCursista(cursista, eventos);



            var relatorio = _relatorioService.GerarPlanilhaRelatorioHistoricoCursistaExcel(vm);

            return new ObjectResult(relatorio);
        }


        public IActionResult Index()
        {
            return View();
        }
	}
}