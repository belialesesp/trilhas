using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Eventos;
using Trilhas.Extensions;
using Trilhas.Helper;
using Trilhas.Helper.Contract;
using Trilhas.Models.Evento;
using Trilhas.Models.Evento.Relatorios;
using Trilhas.Models.Relatorio;

namespace Trilhas.Services
{
    public class RelatorioService
    {
        private readonly FileHelper _fileHelper;

        public RelatorioService(FileHelper fileHelper)
        {

            _fileHelper = fileHelper;
        }

        public DownloadFileContract GerarPlanilhaRelatorioCapacitadosPorPeriodo(List<Evento> eventos)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCapacitadosPorPerido" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("CapacitadosPorPerido");

                int line = 1;
                GenerateHeader(planilha);
                line++;
                var ch = 0;
                var qtdInscritos = 0;
                var qtdAprovados = 0;
                var qtdDeclarados = 0;
                var qtdDesistentes = 0;

                foreach (var evento in eventos)
                {
                    SituacaoEvento situacao = evento.Situacao();
                    string situacaoDisplay = ((System.ComponentModel.DataAnnotations.DisplayAttribute)situacao.GetType().GetMember(situacao.ToString()).First().GetCustomAttributes(false)[0]).Name;

                    planilha.Cell("A" + line).Value = evento.Id;
                    planilha.Cell("B" + line).Value = evento.Curso.Sigla + " - " + evento.Curso.Titulo;
                    planilha.Cell("C" + line).Value = evento.EntidadeDemandante.Sigla;
                    planilha.Cell("D" + line).Value = evento.Curso.CargaHorariaTotal().ToString();
                    planilha.Cell("E" + line).Value = evento.Agendas.LastOrDefault()?.DataHoraInicio.ToString("dd/MM/yyyy")  +  " - " + evento.Agendas.LastOrDefault()?.DataHoraFim.ToString("dd/MM/yyyy");
                    planilha.Cell("F" + line).Value = evento.Curso.Modalidade == EnumModalidade.EAD ? "EAD" : evento.Local.Municipio.NomeMunicipio + "-" + evento.Local.Municipio.Uf;
                    planilha.Cell("G" + line).Value = evento.Curso.Modalidade.GetDescription();
                    planilha.Cell("H" + line).Value = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).Count() : 0;
                    planilha.Cell("I" + line).Value = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.CERTIFICADO).Count() : 0;
                    planilha.Cell("J" + line).Value = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DECLARADO).Count() : 0;
                    planilha.Cell("K" + line).Value = evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DESISTENTE).Count() : 0;
                    planilha.Cell("L" + line).Value = situacaoDisplay;

                    qtdInscritos = qtdInscritos + (evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).Count() : 0);
                    qtdAprovados = qtdAprovados + (evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.CERTIFICADO).Count() : 0);
                    qtdDeclarados = qtdDeclarados  + (evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DECLARADO).Count() : 0);
                    qtdDesistentes = qtdDesistentes + (evento.ListaDeInscricao != null ? evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DESISTENTE).Count() : 0);

                    line++;

                    ch += evento.Curso.CargaHorariaTotal();
                }

                line++;

                planilha.Cell("A" + line).Value = "TOTAIS";
                planilha.Cell("H" + line).Value = qtdInscritos;
                planilha.Cell("I" + line).Value = qtdAprovados;
                planilha.Cell("J" + line).Value = qtdDeclarados;
                planilha.Cell("K" + line).Value = qtdDesistentes;
                planilha.Cell("D" + line).Value = ch;

                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelatorioCapacitadosPorPerido.xlsx");
            }

            static void GenerateHeader(IXLWorksheet worksheet)
            {
                worksheet.Cell("A1").Value = "ID";
                worksheet.Cell("B1").Value = "Curso";
                worksheet.Cell("C1").Value = "Entidade";
                worksheet.Cell("D1").Value = "C/H";
                worksheet.Cell("E1").Value = "Período";
                worksheet.Cell("F1").Value = "Município";
                worksheet.Cell("G1").Value = "Modalidade";
                worksheet.Cell("H1").Value = "Ins";
                worksheet.Cell("I1").Value = "Apr";
                worksheet.Cell("J1").Value = "Dec";
                worksheet.Cell("K1").Value = "Des";
                worksheet.Cell("L1").Value = "Situação";
            }
        }

        public DownloadFileContract GerarPlanilhaRelatorioCapacitadosPorCurso(EventoFinalizadoViewModel evento)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCapacitadosPorCurso" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            int line = 1;


            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("CapacitadosPorCurso");

                planilha.Cell("A" + line).Value = "Curso: ";
                planilha.Cell("B" + line).Value = evento.Nome;

                line++;
                planilha.Cell("A" + line).Value = "Entidade Demandante: ";
                planilha.Cell("B" + line).Value = evento.Entidade;

                line++;
                planilha.Cell("A" + line).Value = "Período de inscrição: ";
                planilha.Cell("B" + line).Value = evento.DataInicioInscricao.ToString("dd/MM/yyyyy") + " - " + evento.DataFimInscricao.ToString("dd/MM/yyyyy");

                line++;
                planilha.Cell("A" + line).Value = "Período de realização: ";
                planilha.Cell("B" + line).Value = evento.DataInicio.ToString("dd/MM/yyyyy") + " - " + evento.DataFim.ToString("dd/MM/yyyyy");


                line++;
                line++;

                planilha.Cell("A" + line).Value = "Nr. Funcional";
                planilha.Cell("B" + line).Value = "CPF";
                planilha.Cell("C" + line).Value = "Docente";


                line++;
                foreach (var docente in evento.Docentes)
                {

                    planilha.Cell("A" + line).Value = docente.NumeroFuncional;
                    planilha.Cell("B" + line).Value = docente.Cpf;
                    planilha.Cell("C" + line).Value = docente.Nome;

                    line++;
                }


                line++;
                planilha.Cell("A" + line).Value = "Nr. Funcional";
                planilha.Cell("B" + line).Value = "CPF";
                planilha.Cell("C" + line).Value = "Cursista";
                planilha.Cell("D" + line).Value = "Situação";
                planilha.Cell("E" + line).Value = "Frequência";

                line++;

                foreach (var inscrito in evento.Inscritos)
                {

                    planilha.Cell("A" + line).Value = inscrito.Cursista.NumeroFuncional;
                    planilha.Cell("B" + line).Value = inscrito.Cursista.Cpf;
                    planilha.Cell("C" + line).Value = inscrito.Cursista.Nome;
                    planilha.Cell("D" + line).Value = inscrito.Situacao;
                    planilha.Cell("E" + line).Value = inscrito.Frequencia;



                    line++;
                }

                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelatorioCapacitadosPorCurso.xlsx");
            }

            static void GenerateHeader(IXLWorksheet worksheet)
            {
                worksheet.Cell("A1").Value = "ID";
                worksheet.Cell("B1").Value = "Curso";
                worksheet.Cell("C1").Value = "Entidade";
                worksheet.Cell("D1").Value = "C/H";
                worksheet.Cell("E1").Value = "Período";
                worksheet.Cell("F1").Value = "Município";
                worksheet.Cell("G1").Value = "Modalidade";
                worksheet.Cell("H1").Value = "Ins";
                worksheet.Cell("I1").Value = "Apr";
                worksheet.Cell("J1").Value = "Dec";
                worksheet.Cell("K1").Value = "Des";
                worksheet.Cell("L1").Value = "Situação";
            }


        }

        public DownloadFileContract GerarPlanilhaRelatorioCursista(ListaInscritosViewModel model)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCursista" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            int line = 1;


            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("CapacitadosPorCurso");

                planilha.Cell("A" + line).Value = "Curso: ";
                planilha.Cell("B" + line).Value = model.Evento.Nome;

                line++;
                planilha.Cell("A" + line).Value = "Entidade Demandante: ";
                planilha.Cell("B" + line).Value = model.Evento.Entidade;

                line++;
                planilha.Cell("A" + line).Value = "Período de inscrição: ";
                planilha.Cell("B" + line).Value = model.Evento.DataInicioInscricao.ToString("dd/MM/yyyyy") + " - " + model.Evento.DataFimInscricao.ToString("dd/MM/yyyyy");

                line++;
                planilha.Cell("A" + line).Value = "Período de realização: ";
                planilha.Cell("B" + line).Value = model.Evento.DataInicio.ToString("dd/MM/yyyyy") + " - " + model.Evento.DataFim.ToString("dd/MM/yyyyy");


                line++;
                line++;

                planilha.Cell("A" + line).Value = "Cursista";
                planilha.Cell("B" + line).Value = "CPF";
                planilha.Cell("C" + line).Value = "Nr.Funcional";
                planilha.Cell("D" + line).Value = "E-mail";
                planilha.Cell("E" + line).Value = "Data Inscrição";



                line++;
                foreach (var _incrito in model.Inscritos)
                {

                    planilha.Cell("A" + line).Value = _incrito.Cursista.Nome;
                    planilha.Cell("B" + line).Value = _incrito.Cursista.Cpf;
                    planilha.Cell("C" + line).Value = _incrito.Cursista.NumeroFuncional;
                    planilha.Cell("D" + line).Value = _incrito.Cursista.Email;
                    planilha.Cell("E" + line).Value = _incrito.DataDeInscricao.ToString("dd/MM/yyyy");





                    line++;
                }



                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelatorioCapacitadosPorCurso.xlsx");
            }

            static void GenerateHeader(IXLWorksheet worksheet)
            {
                worksheet.Cell("A1").Value = "ID";
                worksheet.Cell("B1").Value = "Curso";
                worksheet.Cell("C1").Value = "Entidade";
                worksheet.Cell("D1").Value = "C/H";
                worksheet.Cell("E1").Value = "Período";
                worksheet.Cell("F1").Value = "Município";
                worksheet.Cell("G1").Value = "Modalidade";
                worksheet.Cell("H1").Value = "Ins";
                worksheet.Cell("I1").Value = "Apr";
                worksheet.Cell("J1").Value = "Dec";
                worksheet.Cell("K1").Value = "Des";
                worksheet.Cell("L1").Value = "Situação";
            }


        }

        public DownloadFileContract GerarPlanilhaRelatorioHistoricoCursistaExcel(RelatorioHistoricoCursistaViewModel model)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelHistoricoCursista" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            int line = 1;


            

            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("HistoricoCursista");

                planilha.Cell("A" + line).Value = "Cursista: ";
                planilha.Cell("B" + line).Value = model.Nome;

                line++;
                planilha.Cell("A" + line).Value = "Entidade ";
                planilha.Cell("B" + line).Value = model.Entidade;

                line++;
                planilha.Cell("A" + line).Value = "Endereço";
                planilha.Cell("B" + line).Value = model.Endereco;

                line++;
                planilha.Cell("A" + line).Value = "Contato";
                planilha.Cell("B" + line).Value = model.Telefone;


                line++;
                line++;

                planilha.Cell("A" + line).Value = "Curso";
                planilha.Cell("B" + line).Value = "Entidade";
                planilha.Cell("C" + line).Value = "Periodo";
                planilha.Cell("D" + line).Value = "E-mail";
                planilha.Cell("E" + line).Value = "Data Inscrição";



                line++;
                foreach (var _evento in model.ListaEventos)
                {

                    planilha.Cell("A" + line).Value = _evento.Nome;
                    planilha.Cell("B" + line).Value = _evento.Entidade;
                    planilha.Cell("C" + line).Value = _evento.Docente;
                    planilha.Cell("D" + line).Value = _evento.Periodo;
                    planilha.Cell("E" + line).Value = _evento.Frequencia;
                    planilha.Cell("F" + line).Value = _evento.Resultado;


                    line++;
                }



                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelatorioHistoricoCursista.xlsx");
            }


        }

        public DownloadFileContract GerarPlanilhaRelatorioListaIndividual(RelatorioListaPresencaViewModel model)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCredenciamento" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            int line = 1;


            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("Credenciamento");


                planilha.Cell("A" + line).Value = "Curso: ";
                planilha.Cell("B" + line).Value = model.Evento.EventoNome;

                line++;
                planilha.Cell("A" + line).Value = "Entidade Demandante: ";
                planilha.Cell("B" + line).Value = model.Evento.EntidadeNome;

                line++;
                planilha.Cell("A" + line).Value = "Local: ";
                planilha.Cell("B" + line).Value = model.Evento.LocalNome;

                planilha.Cell("C" + line).Value = "Data: ";
                planilha.Cell("D" + line).Value = model.DataInicio.ToString("dd/MM/yyyy");

                planilha.Cell("E" + line).Value = "Hora: ";
                planilha.Cell("F" + line).Value = model.DataInicio.ToString("HH:mm") + " às " + model.DataFim.ToString("HH:mm");

                line++;
                line++;


                planilha.Cell("A" + line).Value = "NFUNC";
                planilha.Cell("B" + line).Value = "Nome";
                planilha.Cell("C" + line).Value = "CPF";
                planilha.Cell("D" + line).Value = "Entidade";
                planilha.Cell("E" + line).Value = "Assinatura";

                line++;


                foreach (var inscrito in model.ListaInscritos)
                {

                    planilha.Cell("A" + line).Value = inscrito.NumeroFuncional;
                    planilha.Cell("B" + line).Value = inscrito.CursistaNome;
                    planilha.Cell("C" + line).Value = inscrito.CursistaCPF;
                    planilha.Cell("D" + line).Value = inscrito.EntidadeSigla;


                    line++;
                }

                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelCredenciamento.xlsx");
            }
        }

        public DownloadFileContract GerarPlanilhaRelatorioListaCompleta(RelatorioListaPresencaViewModel model)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCredenciamento" + DateTime.Now.ToString("yyyyMMddHHmmss")  + ".xlsx";

            if (File.Exists(filePathName))
                File.Delete(filePathName);

            int line = 1;


            using (var workbook = new XLWorkbook())
            {
                var planilha = workbook.Worksheets.Add("Credenciamento");

                foreach (var data in model.Datas)
                {
                    planilha.Cell("A" + line).Value = "Curso: ";
                    planilha.Cell("B" + line).Value = model.Evento.EventoNome;

                    line++;
                    planilha.Cell("A" + line).Value = "Entidade Demandante: ";
                    planilha.Cell("B" + line).Value = model.Evento.EntidadeNome;

                    line++;
                    planilha.Cell("A" + line).Value = "Local: ";
                    planilha.Cell("B" + line).Value = model.Evento.LocalNome;

                    planilha.Cell("C" + line).Value = "Data: ";
                    planilha.Cell("D" + line).Value = data.Data;

                    planilha.Cell("E" + line).Value = "Hora: ";
                    planilha.Cell("F" + line).Value = data.HoraInicio.ToString("HH:mm") + " às " + data.HoraFim.ToString("HH:mm");

                    line++;
                    line++;


                    planilha.Cell("A" + line).Value = "NFUNC";
                    planilha.Cell("B" + line).Value = "Nome";
                    planilha.Cell("C" + line).Value = "CPF";
                    planilha.Cell("D" + line).Value = "Entidade";
                    planilha.Cell("E" + line).Value = "Assinatura";

                    line++;


                    foreach (var inscrito in model.ListaInscritos)
                    {

                        planilha.Cell("A" + line).Value = inscrito.NumeroFuncional;
                        planilha.Cell("B" + line).Value = inscrito.CursistaNome;
                        planilha.Cell("C" + line).Value = inscrito.CursistaCPF;
                        planilha.Cell("D" + line).Value = inscrito.EntidadeSigla;




                        line++;
                    }

                    line++;
                    line++;


                }



                workbook.SaveAs(filePathName);

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName, "RelCredenciamento.xlsx");
            }


        }




    }
}
