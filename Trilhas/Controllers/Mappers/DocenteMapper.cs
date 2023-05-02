using System;
using System.Collections.Generic;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros.Docente;
using Trilhas.SqlDto;

namespace Trilhas.Controllers.Mappers
{
    public class DocenteMapper
    {
        public List<GridDocenteViewModel> MapearGridDocente(List<Docente> docentes)
        {
            var gridDocentesVm = new List<GridDocenteViewModel>();

            foreach (var docente in docentes)
            {
                gridDocentesVm.Add(new GridDocenteViewModel
                {
                    Nome = docente.Pessoa.NomeSocial ?? docente.Pessoa.Nome,
                    Email = docente.Pessoa.Email,
                    CPF = FormatadorDeDados.FormatarCPF(docente.Pessoa.Cpf),
                    Id = docente.Id,
                    Excluido = docente.DeletionTime.HasValue,
                });
            }

            return gridDocentesVm;
        }

        public List<GridDocenteViewModel> MapearDadosGridDocente(List<GridDocenteDto> docentes)
        {
            var gridDocentesVm = new List<GridDocenteViewModel>();

            foreach (var docente in docentes)
            {
                gridDocentesVm.Add(new GridDocenteViewModel
                {
                    Id = docente.Id,
                    Nome = docente.Nome,
                    CPF = FormatadorDeDados.FormatarCPF(docente.CPF),
                    Email = docente.Email,
                    Excluido = Convert.ToBoolean(docente.Excluido),
                    CargaHorariaTotal = docente.CargaHorariaTotal,
                    QuantidadeEnvento = docente.QuantidadeEvento
                });
            }

            return gridDocentesVm;
        }

        public DocenteViewModel MapearDocente(Docente docente)
        {
            var dadosBancarios = new List<DocenteDadosBancariosViewModel>();
            var habilitacoes = new List<DocenteHabilitacaoViewModel>();
            var formacoes = new List<DocenteFormacaoViewModel>();

            if (docente.DadosBancarios != null)
            {
                foreach (var dadoBancario in docente.DadosBancarios)
                {
                    dadosBancarios.Add(new DocenteDadosBancariosViewModel
                    {
                        Agencia = dadoBancario.Agencia,
                        Banco = dadoBancario.Banco,
                        ContaCorrente = dadoBancario.ContaCorrente,
                        Id = dadoBancario.Id
                    });
                }
            }

            if (docente.Habilitacao != null)
            {
                foreach (var habilitacao in docente.Habilitacao)
                {
                    habilitacoes.Add(new DocenteHabilitacaoViewModel
                    {
                        Curso = new DocenteCursoViewModel
                        {
                            CargaHoraria = habilitacao.Curso.CargaHorariaTotal(),
                            Titulo = habilitacao.Curso.Sigla + " - " + habilitacao.Curso.Titulo,
                            Id = habilitacao.Curso.Id,
                            Modalidade = habilitacao.Curso.Modalidade.ToString()
                        },
                        Id = habilitacao.Id,
                    });
                }
            }

            if (docente.Formacao != null)
            {
                foreach (var formacao in docente.Formacao)
                {
                    formacoes.Add(new DocenteFormacaoViewModel
                    {
                        CargaHoraria = formacao.CargaHoraria,
                        Curso = formacao.Curso,
                        DataFim = formacao.DataFim,
                        DataInicio = formacao.DataInicio,
                        Id = formacao.Id,
                        Instituicao = formacao.Instituicao,
                        Titulacao = formacao.Titulacao
                    });
                }
            }

            var docenteVm = new DocenteViewModel
            {
                DadosBancarios = dadosBancarios,
                Habilitacao = habilitacoes,
                Formacao = formacoes,
                Id = docente.Id,
                Observacoes = docente.Observacoes,
                PessoaId = docente.Pessoa.Id,
                Pis = docente.Pessoa.Pis,
                Titulo = docente.Pessoa.NumeroTitulo,
                Cpf = docente.Pessoa.Cpf,
                Nome = docente.Pessoa.NomeSocial ?? docente.Pessoa.Nome,
                Excluido = docente.DeletionTime.HasValue
            };

            return docenteVm;
        }
    }
}
