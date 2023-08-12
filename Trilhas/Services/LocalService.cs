using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Migrations;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Services
{
    public class LocalService
    {
        private ApplicationDbContext _context;

        public LocalService(ApplicationDbContext context)
        {
            _context = context;
        }

        //LOCAIS
        public Local SalvarLocal(string userId, Local local, List<LocalSala> salas, List<LocalRecurso> recursos, List<LocalContato> contatos)
        {
            var tx = _context.Database.BeginTransaction();

            try
            {
                if (local.Id > 0)
                {
                    SalvarSalas(local, salas, userId);
                    recursos = AgruparRecursos(recursos);
                    SalvarRecursos(local, recursos, userId);
                    SalvarContatos(local, contatos, userId);

                    local.LastModifierUserId = userId;
                    local.LastModificationTime = DateTime.Now;
                }
                else
                {
                    local.Salas = salas;
                    local.Recursos = recursos;
                    local.Contatos = contatos;

                    DateTime agora = DateTime.Now;

                    local.CreatorUserId = userId;
                    local.CreationTime = agora;

                    foreach (var sala in local.Salas)
                    {
                        sala.CreatorUserId = userId;
                        sala.CreationTime = agora;
                    }

                    foreach (var recurso in local.Recursos)
                    {
                        recurso.CreatorUserId = userId;
                        recurso.CreationTime = agora;
                    }

                    foreach (var contato in local.Contatos)
                    {
                        contato.CreatorUserId = userId;
                        contato.CreationTime = agora;
                    }

                    _context.Locais.Add(local);
                }

                _context.SaveChanges();
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }

            return local;
        }

        /*----------SALAS------------*/
        private void SalvarSalas(Local local, List<LocalSala> salasAtualizadas, string userId)
        {
            var salas = RecuperarSalas(local.Id);

            AtualizarOuExcluirSalas(salas, salasAtualizadas, userId);
            AdicionarNovasSalas(salas, salasAtualizadas, userId);

            _context.SaveChanges();
        }

        private void AtualizarOuExcluirSalas(List<LocalSala> salas, List<LocalSala> salasAtualizadas, string userId)
        {
            var salasExcluidas = salas.Where(o => !salasAtualizadas.Select(x => x.Id).Contains(o.Id)).ToList();

            salasExcluidas.RemoveAll(x => _context.EventoHorario.Include(i => i.Evento).Any(y => y.Sala.Id == x.Id && y.DataHoraInicio >= DateTime.Now && !y.Evento.DeletionTime.HasValue));

            foreach (var sala in salas)
            {
                var salaAtualizada = salasAtualizadas.FirstOrDefault(x => x.Id == sala.Id);

                if (salaAtualizada != null)
                {
                    AtualizarCadastroSala(sala, salaAtualizada, userId);
                }

                if (salasExcluidas.Any(x => x.Id == sala.Id))
                {
                    ExcluirEntidade(sala, userId);
                }
            }
        }

        private void AdicionarNovasSalas(List<LocalSala> salas, List<LocalSala> salasAtualizadas, string userId)
        {
            var novasSalas = salasAtualizadas.Where(x => !salas.Any(y => y.Id == x.Id));

            foreach (var sala in novasSalas)
            {
                sala.CreationTime = DateTime.Now;
                sala.CreatorUserId = userId;

                _context.LocalSalas.Add(sala);
            }
        }

        private void AtualizarCadastroSala(LocalSala salaCadastrada, LocalSala salaAtualizada, string userId)
        {
            salaCadastrada.Numero = salaAtualizada.Numero;
            salaCadastrada.Sigla = salaAtualizada.Sigla;
            salaCadastrada.Numero = salaAtualizada.Numero;

            salaCadastrada.LastModificationTime = DateTime.Now;
            salaCadastrada.LastModifierUserId = userId;
        }

        /*----------RECURSOS------------*/
        private List<LocalRecurso> AgruparRecursos(List<LocalRecurso> recursos)
        {
            List<LocalRecurso> recursosAgrupados = new List<LocalRecurso>();

            foreach (var localRecurso in recursos)
            {
                var item = recursosAgrupados.FirstOrDefault(x => x.Recurso.Id == localRecurso.Recurso.Id);

                if (item == null)
                {
                    recursosAgrupados.Add(localRecurso);
                }
                else
                {
                    item.Quantidade += localRecurso.Quantidade;
                }
            }

            return recursosAgrupados;
        }

        private void SalvarRecursos(Local local, List<LocalRecurso> recursosAtualiados, string userId)
        {
            var recursos = RecuperarRecursos(local.Id);

            AtualizarOuExcluirRecursos(recursos, recursosAtualiados, userId);
            AdicionarNovosRecursos(recursos, recursosAtualiados, userId);

            _context.SaveChanges();
        }

        private void AtualizarOuExcluirRecursos(List<LocalRecurso> recursos, List<LocalRecurso> recursosAtualizados, string userId)
        {
            var recursosExcluidos = recursos.Where(o => !recursosAtualizados.Select(x => x.Id).Contains(o.Id)).ToList();

            foreach (var recurso in recursos)
            {
                var recursoAtualizado = recursosAtualizados.FirstOrDefault(x => x.Id == recurso.Id);

                if (recursoAtualizado != null)
                {
                    AtualizarLocalRecurso(recurso, recursoAtualizado, userId);
                }

                if (recursosExcluidos.Any(x => x.Id == recurso.Id))
                {
                    ExcluirEntidade(recurso, userId);
                }
            }
        }

        private void AdicionarNovosRecursos(List<LocalRecurso> recursos, List<LocalRecurso> recursosAtualizados, string userId)
        {
            var novosRecursos = recursosAtualizados.Where(x => !recursos.Any(y => y.Id == x.Id));

            foreach (var recurso in novosRecursos)
            {
                recurso.CreationTime = DateTime.Now;
                recurso.CreatorUserId = userId;

                _context.LocalRecursos.Add(recurso);
            }
        }

        private void AtualizarLocalRecurso(LocalRecurso recursoCadastrado, LocalRecurso recursoAtualizado, string userId)
        {
            recursoCadastrado.Recurso = recursoAtualizado.Recurso;
            recursoCadastrado.Quantidade = recursoAtualizado.Quantidade;
            recursoCadastrado.LastModificationTime = DateTime.Now;
            recursoCadastrado.LastModifierUserId = userId;
        }

        /*----------CONTATOS------------*/
        private void SalvarContatos(Local local, List<LocalContato> contatosAtualiados, string userId)
        {
            var contatos = RecuperarContatos(local.Id);

            AtualizarOuExcluirContatos(contatos, contatosAtualiados, userId);
            AdicionarNovosContatos(contatos, contatosAtualiados, userId);

            _context.SaveChanges();
        }

        private void AtualizarOuExcluirContatos(List<LocalContato> contatos, List<LocalContato> contatosAtualizados, string userId)
        {
            var contatosExcluidos = contatos.Where(o => !contatosAtualizados.Select(x => x.Id).Contains(o.Id)).ToList();

            foreach (var contato in contatos)
            {
                var contatoAtualizado = contatosAtualizados.FirstOrDefault(x => x.Id == contato.Id);

                if (contatoAtualizado != null)
                {
                    AtualizarLocalContato(contato, contatoAtualizado, userId);
                }

                if (contatosExcluidos.Any(x => x.Id == contato.Id))
                {
                    ExcluirEntidade(contato, userId);
                }
            }
        }

        private void AdicionarNovosContatos(List<LocalContato> contatos, List<LocalContato> contatosAtualizados, string userId)
        {
            var novosContatos = contatosAtualizados.Where(x => !contatos.Any(y => y.Id == x.Id));

            foreach (var contato in novosContatos)
            {
                contato.CreationTime = DateTime.Now;
                contato.CreatorUserId = userId;

                _context.LocalContatos.Add(contato);
            }
        }

        private void AtualizarLocalContato(LocalContato contatoCadastrado, LocalContato contatoAtualizado, string userId)
        {
            contatoCadastrado.NumeroTelefone = contatoAtualizado.NumeroTelefone;
            contatoCadastrado.TipoContato = contatoAtualizado.TipoContato;

            contatoCadastrado.LastModificationTime = DateTime.Now;
            contatoCadastrado.LastModifierUserId = userId;
        }

        private void ExcluirEntidade(DefaultEntity entidade, string userId)
        {
            entidade.DeletionTime = DateTime.Now;
            entidade.DeletionUserId = userId;
        }

        /*----------------------*/


        public void ExcluirLocal(string userId, long id)
        {
            Local local = RecuperarLocalCompleto(id);

            if (local == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirLocal(local))
            {
                throw new ConstraintException("O Local está vinculado a algum Evento.");
            }

            local.DeletionTime = DateTime.Now;
            local.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool PodeExcluirLocal(Local local)
        {
            return !_context.Eventos
                .Include(x => x.Local)
                .Any(x => x.Local.Id == local.Id && !x.DeletionTime.HasValue);
        }

        public Local RecuperarLocal(long id, bool incluirExcluidos = false)
        {
            return _context.Locais
                .Include(x => x.Municipio)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public Local RecuperarLocalCompleto(long id, bool incluirExcluidos = false)
        {
            return _context.Locais
                .Include(x => x.Municipio)
                .Include(x => x.Salas)
                .Include(x => x.Recursos).ThenInclude(x => x.Recurso)
                .Include(x => x.Contatos).ThenInclude(x => x.TipoContato)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public int QuantidadeDeLocais(string nome, int capacidade, string endereco, bool excluidos)
        {
            return PesquisarLocal(nome, capacidade, endereco, excluidos).AsEnumerable().Count();
        }

        public List<Local> PesquisarLocais(string nome, int capacidade, string endereco, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarLocal(nome, capacidade, endereco, excluidos, start, count).AsEnumerable().ToList();
        }

        public bool VerificarSalaAlocada(LocalSala sala)
        {
            return _context.EventoHorario.Include(i => i.Evento).Any(y => y.Sala.Id == sala.Id && y.DataHoraInicio >= DateTime.Now && !y.Evento.DeletionTime.HasValue);
        }

        private IQueryable<Local> PesquisarLocal(string nome, int capacidade, string endereco, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Local> result = _context.Locais;
               // .Include(x => x.Municipio)
               // .Include(x => x.Contatos)
               // .Include(x => x.Recursos)
               // .Include(x => x.Salas);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (capacidade > 0)
            {
                result = result.Where(x => x.Salas.Sum(y => y.Capacidade) >= capacidade);
            }
            if (!string.IsNullOrEmpty(endereco))
            {
                //List<Municipio> municipios = _context.Municipios.Where(x => x.NomeMunicipio.Contains(endereco)).ToList();
                result = result.Where(x => (x.Bairro.ToUpper().Contains(endereco.Trim().ToUpper()))
                            || (x.Municipio.NomeMunicipio.ToUpper().Contains(endereco.Trim().ToUpper()))
                            || (x.Logradouro.ToUpper().Contains(endereco.Trim().ToUpper()))
                            || (x.Municipio.Uf.ToUpper().Contains(endereco.Trim().ToUpper())));
            }

            result = result.OrderBy(x => x.Nome);

            if (start > 0)
            {
                result = result.Skip(start);
            }
            if (count > 0)
            {
                result = result.Take(count);
            }

            return result;
        }

        public LocalRecurso RecuperarLocalRecurso(long idLocalRecurso)
        {
            return _context.LocalRecursos
                .Include(x => x.Recurso)
                .Include(x => x.Local)
                .Where(x => x.Id == idLocalRecurso)
                .FirstOrDefault();
        }

        public LocalContato RecuperarLocalContato(long idLocalContato)
        {
            return _context.LocalContatos
                .Include(x => x.TipoContato)
                .Include(x => x.Local)
                .Where(x => x.Id == idLocalContato)
                .FirstOrDefault();
        }

        public List<LocalSala> RecuperarSalas(long idLocal)
        {
            return _context.LocalSalas
                .Where(x => x.Local.Id == idLocal && x.DeletionTime == null)
                .ToList();
        }

        public List<LocalRecurso> RecuperarRecursos(long idLocal)
        {
            return _context.LocalRecursos
                .Where(x => x.Local.Id == idLocal && x.DeletionTime == null)
                .ToList();
        }

        public List<LocalContato> RecuperarContatos(long idLocal)
        {
            return _context.LocalContatos
                .Where(x => x.Local.Id == idLocal && x.DeletionTime == null)
                .ToList();
        }

        public LocalSala RecuperarLocalSala(long idLocalSala)
        {
            return _context.LocalSalas
                .Include(x => x.Local)
                .Where(x => x.Id == idLocalSala).FirstOrDefault();
        }

        public List<TipoLocalContato> RecuperarTiposLocalContato()
        {
            return _context.TipoLocalContato.ToList();
        }

        public TipoLocalContato RecuperarTipoLocalContato(long id)
        {
            return _context.TipoLocalContato.FirstOrDefault(x => x.Id == id);
        }
    }
}
