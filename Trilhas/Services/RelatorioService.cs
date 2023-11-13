using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Eventos;
using Trilhas.Extensions;
using Trilhas.Helper;
using Trilhas.Helper.Contract;

namespace Trilhas.Services
{
    public class RelatorioService
    {
        private readonly FileHelper _fileHelper;

        public RelatorioService(FileHelper fileHelper) {

            _fileHelper = fileHelper;
        }

        public DownloadFileContract GerarPlanilhaRelatorioCapacitadosPorPeriodo(List<Evento> eventos)
        {
            string filePathName = _fileHelper.GetAppDataPath() + "RelCapacitadosPorPerido.xlsx";

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

                return _fileHelper.ObterBytesDoArquivoParaDownload(filePathName);
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


    }
}
