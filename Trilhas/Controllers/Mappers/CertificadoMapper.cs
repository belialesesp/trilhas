using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Trilhas.Data.Enums;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Certificado;
using Trilhas.Models.Relatorio;

namespace Trilhas.Controllers.Mappers
{
    public class CertificadoMapper
    {
        public List<CertificadoViewModel> MapearGridCertificado(List<Certificado> certificados)
        {
            List<CertificadoViewModel> certificadoVm = new List<CertificadoViewModel>();

            foreach (var certificado in certificados)
            {
                certificadoVm.Add(new CertificadoViewModel(certificado.Id)
                {
                    Nome = certificado.Nome,
                    Dados = certificado.Dados,
                    Padrao = certificado.Padrao,
                    Excluido = certificado.DeletionTime.HasValue,
                    TipoCertificado = certificado.TipoCertificado
                });
            }

            return certificadoVm;
        }

        public CertificadoViewModel MapearCeritificado(Certificado certificado)
        {
            CertificadoViewModel vm = new CertificadoViewModel(certificado.Id)
            {
                Nome = certificado.Nome,
                Dados = certificado.Dados,
                Padrao = certificado.Padrao,
                Excluido = certificado.DeletionTime.HasValue,
                TipoCertificado = certificado.TipoCertificado
            };

            return vm;
        }

        public EmissaoCertificadoViewModel MaperPreviewCertificado(Certificado certificado)
        {
            EmissaoCertificadoViewModel vm = new EmissaoCertificadoViewModel();

            if (certificado == null)
            {
                return vm;
            }

            vm.Dados = certificado.Dados;
            vm.Dados = vm.Dados.Replace("[#DATA_ATUAL]", GerarDataPorExtenso());

            return vm;
        }

        public EmissaoCertificadoViewModel MapearEmissaoCertificado(Inscrito inscrito, string dados)
        {
            EmissaoCertificadoViewModel vm = new EmissaoCertificadoViewModel();

            if (inscrito == null || inscrito.ListaDeInscricao.Evento.Certificado == null)
            {
                return vm;
            }

            var horario = inscrito.ListaDeInscricao.Evento.Horarios.First();
            dados = dados.Replace("[#TITULO_CURSO]", inscrito.ListaDeInscricao.Evento.Curso.Titulo);
            dados = dados.Replace("[#NOME_DOCENTE]", horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.NomeSocial);
            dados = dados.Replace("[#CONTEUDO_PROGRAMATICO]", inscrito.ListaDeInscricao.Evento.Curso.ConteudoProgramatico);
            dados = dados.Replace("[#CARGA_HORARIA]", inscrito.ListaDeInscricao.Evento.Curso.CargaHorariaTotal().ToString());
            dados = dados.Replace("[#DATA_INICIAL]", inscrito.ListaDeInscricao.Evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy"));
            dados = dados.Replace("[#DATA_FINAL]", inscrito.ListaDeInscricao.Evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"));
            dados = dados.Replace("[#NOME_CURSISTA]", inscrito.Cursista.NomeSocial ?? inscrito.Cursista.Nome);

            dados = dados.Replace("[#LOCAL]", inscrito.ListaDeInscricao.Evento.Local != null ? inscrito.ListaDeInscricao.Evento.Local.Nome : string.Empty);

            dados = dados.Replace("[#DATA_ATUAL]", GerarDataPorExtenso());

            vm.Dados = dados;
            vm.Tipo = inscrito.Situacao == EnumSituacaoCursista.CERTIFICADO ? "Certificado" : "Declaração";

            return vm;
        }

        public EmissaoCertificadoViewModel MapearEmissaoDeclaracaoDocente(Docente docente, Evento evento, string dados)
        {
            EmissaoCertificadoViewModel vm = new EmissaoCertificadoViewModel();

            var horario = evento.Horarios.First();
            dados = dados.Replace("[#TITULO_CURSO]", evento.Curso.Titulo);
            dados = dados.Replace("[#NOME_DOCENTE]", docente.Pessoa.Nome);
            dados = dados.Replace("[#CONTEUDO_PROGRAMATICO]", evento.Curso.ConteudoProgramatico);
            dados = dados.Replace("[#CARGA_HORARIA]", evento.Curso.CargaHorariaTotal().ToString());
            dados = dados.Replace("[#DATA_INICIAL]", evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy"));
            dados = dados.Replace("[#DATA_FINAL]", evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"));

            dados = dados.Replace("[#LOCAL]", evento.Local != null ? evento.Local.Nome : string.Empty);

            dados = dados.Replace("[#DATA_ATUAL]", GerarDataPorExtenso());

            vm.Dados = dados;
            vm.Tipo = "Declaração";

            return vm;
        }

        public RelatorioCursistaViewModel MapearRelatorioCursista(Evento evento)
        {
            RelatorioCursistaViewModel vm = new RelatorioCursistaViewModel();
			vm.Relatorio = "RELATÓRIO DE INSCRITOS";
            vm.Nome = evento.Curso.Sigla + " - " + evento.Curso.Titulo;

            vm.Local = (evento.Curso.Modalidade == Data.Enums.EnumModalidade.EAD) ? "EAD" : evento.Local.Nome;

            vm.Periodo = evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy") + " - " + evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy");
            vm.Entidade = evento.Curso.Sigla + " - " + evento.EntidadeDemandante.Nome;
            vm.Horario = evento.Agenda().DataHoraInicio.ToString("HH:mm") + " - " + evento.Agenda().DataHoraFim.ToString("HH:mm");
            vm.DataAtual = DateTime.Now;

            var lista = evento.ListaDeInscricao.PessoasInscritas();

            foreach (var inscrito in lista)
            {
                vm.ListaInscritos.Add(new RelatorioCursistaInscritosViewModel
                {
                    NumeroFuncional = inscrito.NumeroFuncional,
                    Nome = inscrito.NomeSocial ?? inscrito.Nome,
                    Cpf = FormatadorDeDados.FormatarCPF(inscrito.Cpf),
                    Email = inscrito.Email,
                    Endereco = inscrito.Municipio.NomeMunicipio + "-" + inscrito.Municipio.Uf,
                    Telefone = FormatadorDeDados.FormatarTelefone(inscrito.Contatos.First().Numero)
                });
            }

            vm.ListaInscritos = vm.ListaInscritos.OrderBy(x => x.Nome).ToList();

            return vm;
        }
        private string GerarDataPorExtenso()
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;

            int dia = DateTime.Now.Day;
            int ano = DateTime.Now.Year;
            string mes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(DateTime.Now.Month));
            string diasemana = culture.TextInfo.ToTitleCase(dtfi.GetDayName(DateTime.Now.DayOfWeek));
            string data = diasemana + ", " + dia + " de " + mes + " de " + ano;

            return data;
        }

        public RelatorioHistoricoCursistaViewModel MapearRelatorioHistoricoCursista(Pessoa cursista, List<Evento> eventos)
        {
            RelatorioHistoricoCursistaViewModel vm = new RelatorioHistoricoCursistaViewModel();
            vm.DataAtual = DateTime.Now;
			vm.Relatorio = "RELATÓRIO HISTÓRICO DE CURSISTA";
			vm.NumeroFuncional = cursista.NumeroFuncional;
            vm.Nome = cursista.NomeSocial ?? cursista.Nome;
            vm.Email = cursista.Email;
            vm.Endereco = cursista.Municipio.NomeMunicipio + "-" + cursista.Municipio.Uf;
            vm.Entidade = cursista.Entidade.Sigla + " - " + cursista.Entidade.Nome;
            vm.Telefone = FormatadorDeDados.FormatarTelefone(cursista.Contatos.First().Numero);

            foreach (var evento in eventos)
            {
                var horario = evento.Horarios.First();

                vm.ListaEventos.Add(new RelatorioHistoricoCursistaEventoViewModel
                {
                    Docente = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome,
                    Nome = evento.Curso.Sigla + "-" + evento.Curso.Titulo,
                    Entidade = evento.EntidadeDemandante.Sigla,
                    Frequencia = evento.ListaDeInscricao.Inscritos.FirstOrDefault(x => x.Cursista.Id == cursista.Id).Frequencia.ToString() + " %",
                    Periodo = evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy") + " - " + evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"),
                    Resultado = evento.ListaDeInscricao.Inscritos.FirstOrDefault(x => x.Cursista.Id == cursista.Id).Situacao.ToString()
                });
            }

            vm.ListaEventos = vm.ListaEventos.OrderBy(x => x.Nome).ToList();

            return vm;
        }
    }
}
