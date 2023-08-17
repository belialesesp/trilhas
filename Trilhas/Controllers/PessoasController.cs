using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Models.Cadastros.Pessoa;
using Trilhas.Services;
using Trilhas.SqlDto;

namespace Trilhas.Controllers
{
    public class PessoasController : DefaultController
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly CadastroService _cadastroService;
        private readonly PessoaService _pessoaService;
        private readonly EntidadeService _entidadeService;
        private readonly PessoaMapper _mapper;

        public PessoasController(UserManager<IdentityUser> userManager, CadastroService service, PessoaService pessoaService, EntidadeService entidadeService) : base(userManager)
        {
            _pessoaService = pessoaService;
            _entidadeService = entidadeService;
            _cadastroService = service;
            _mapper = new PessoaMapper();
        }

        [HttpGet]
        public IActionResult Quantidade(string nome, string email, string cpf, string numeroFuncional, long entidadeId, bool excluidos)
        {
            int qtd = _pessoaService.QuantidadeDePessoas(nome, email, cpf, numeroFuncional, entidadeId, 0, excluidos);
            return new ObjectResult(qtd);
        }

        [HttpGet]
        public IActionResult Buscar(string nome, string email, string cpf, string numeroFuncional, long entidadeId, bool excluidos, int start = -1, int count = -1)
        {
            List<Pessoa> pessoas = _pessoaService.PesquisarPessoas(nome, email, cpf, numeroFuncional, entidadeId, 0, excluidos, start, count);

            var vm = _mapper.MapearPessoasViewModel(pessoas);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Pesquisar(string nome, string cpf, string numeroFuncional, int start = -1, int count = -1)
        {
            List<Pessoa> pessoas = _pessoaService.PesquisarPessoas(nome, null, cpf, numeroFuncional, 0, 0, false, start, count);

            var vm = _mapper.MapearPessoasModalViewModel(pessoas);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Pessoa pessoa = _pessoaService.RecuperarPessoaCompleto(id, true);

            var vm = _mapper.MapearPessoaViewModel(pessoa);

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RecuperarBasico(long id)
        {
            Pessoa pessoa = _pessoaService.RecuperarPessoa(id, true);

            var vm = _mapper.MapearPessoaViewModel(pessoa);

            return Json(vm);
        }

        [HttpGet]
		public IActionResult BuscarCursistas(long? cursista, long? curso, long? modalidade, long? entidade, string uf, long? municipio, DateTime? dataInicio, DateTime? dataFim, bool desistentes, int start = -1, int count = -1)
		{
			var cursistas = _pessoaService.PesquisarCursistasSqlQuery(cursista, curso, modalidade, entidade, uf, municipio, dataInicio, dataFim, desistentes, start, count);

			var vm = _mapper.MapearCursistasViewModel(cursistas);

            var x = vm.AsQueryable();

            if (start > 0)
            {
                x = x.Skip(start);
            }
            if (count > 0)
            {
                x = x.Take(count);
            }

            return Json(x.ToList());
        }

		[HttpGet]
		public IActionResult QuantidadeCursistaGrid(long? cursista, long? curso, long? modalidade, long? entidade, string uf, long? municipio, DateTime? dataInicio, DateTime? dataFim, bool desistentes, int start = -1, int count = -1)
		{
			var qtd = _pessoaService.PesquisarQuantidadeCursistasSqlQuery(cursista, curso, modalidade, entidade, uf, municipio, dataInicio, dataFim, desistentes);
			return new ObjectResult(qtd);
		}	

		[HttpGet]
        public IActionResult RecuperarPessoaPorCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return Json("");
            }

            Pessoa pessoa = _pessoaService.RecuperarPessoaPorCpf(cpf);

            if (pessoa == null)
            {
                return BadRequest("CPF não encontrado.");
            }

            var vm = _mapper.MapearPessoaBuscaPorCpfViewModel(pessoa);

            return Json(vm);
        }

        [HttpDelete]
        public IActionResult Excluir(long id)
        {
            try
            {
                _pessoaService.ExcluirPessoa(RecuperarUsuarioId(), id);
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
        public IActionResult Salvar([FromBody] SalvarPessoaViewModel vm)
        {
            try
            {
                ValidarCadastroPessoa(vm);

                Pessoa pessoa;

                if (vm.Id > 0)
                {
                    pessoa = AtualizarPessoa(vm);
                }
                else
                {
                    pessoa = CriarPessoa(vm);
                }

                pessoa = _pessoaService.SalvarPessoa(RecuperarUsuarioId(), pessoa);

                return JsonFormResponse(pessoa.Id);
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

        private Pessoa CriarPessoa(SalvarPessoaViewModel vm)
        {
            Deficiencia deficiencia = vm.Deficiencia.HasValue ? _pessoaService.RecuperarDeficiencia(vm.Deficiencia.Value) : null;
            Escolaridade escolaridade = _pessoaService.RecuperarEscolaridade(vm.Escolaridade);
            OrgaoExpedidor orgaoExpedidor = _pessoaService.RecuperarOrgaoExpedidor(vm.OrgaoExpedidorIdentidade);
            Entidade entidade = _entidadeService.RecuperarEntidade(vm.EntidadeId);
            Municipio cidade = _cadastroService.RecuperarMunicipio(vm.Cidade);
            Sexo sexo = _pessoaService.RecuperarSexo(vm.Sexo);

            Pessoa pessoa = new Pessoa
            {
                Nome = vm.Nome,
                NomeSocial = vm.NomeSocial,
                Sexo = sexo,
                Logradouro = vm.Logradouro,
                Bairro = vm.Bairro,
                Cep = vm.Cep,
                Municipio = cidade,
                Complemento = vm.Complemento,
                Cpf = FormatadorDeDados.RemoverFormatacao(vm.Cpf),
                DataNascimento = vm.DataNascimento,
                FlagDeficiente = vm.FlagDeficiente,
                Deficiencia = deficiencia,
                Escolaridade = escolaridade,
                OrgaoExpedidorIdentidade = orgaoExpedidor,
                Entidade = entidade,
                NumeroFuncional = vm.NumeroFuncional,
                NumeroIdentidade = vm.NumeroIdentidade,
                Numero = vm.Numero,
                NumeroTitulo = vm.NumeroTitulo,
                Pis = vm.Pis,
                UfIdentidade = vm.UfIdentidade,
                Email = vm.Email,
                Imagem = vm.Imagem,
                Contatos = new List<PessoaContato>()
            };

            pessoa.Contatos = SalvarListaPessoaContato(pessoa, vm.Contatos);

            return pessoa;
        }

        private Pessoa AtualizarPessoa(SalvarPessoaViewModel vm)
        {
            Pessoa pessoa = _pessoaService.RecuperarPessoaCompleto(vm.Id);

            Deficiencia deficiencia = vm.Deficiencia.HasValue ? _pessoaService.RecuperarDeficiencia(vm.Deficiencia.Value) : null;
            Escolaridade escolaridade = _pessoaService.RecuperarEscolaridade(vm.Escolaridade);
            OrgaoExpedidor orgaoExpedidor = _pessoaService.RecuperarOrgaoExpedidor(vm.OrgaoExpedidorIdentidade);
            Entidade entidade = _entidadeService.RecuperarEntidade(vm.EntidadeId);
            Municipio cidade = _cadastroService.RecuperarMunicipio(vm.Cidade);
            Sexo sexo = _pessoaService.RecuperarSexo(vm.Sexo);

            pessoa.Nome = vm.Nome;
            pessoa.NomeSocial = vm.NomeSocial;
            pessoa.Sexo = sexo;
            pessoa.Logradouro = vm.Logradouro;
            pessoa.Bairro = vm.Bairro;
            pessoa.Cep = vm.Cep;
            pessoa.Municipio = cidade;
            pessoa.Complemento = vm.Complemento;
            pessoa.Cpf = FormatadorDeDados.RemoverFormatacao(vm.Cpf);
            pessoa.DataNascimento = vm.DataNascimento;
            pessoa.Deficiencia = deficiencia;
            pessoa.Escolaridade = escolaridade;
            pessoa.OrgaoExpedidorIdentidade = orgaoExpedidor;
            pessoa.Entidade = entidade;
            pessoa.FlagDeficiente = vm.FlagDeficiente;
            pessoa.NumeroFuncional = vm.NumeroFuncional;
            pessoa.NumeroIdentidade = vm.NumeroIdentidade;
            pessoa.Numero = vm.Numero;
            pessoa.NumeroTitulo = vm.NumeroTitulo;
            pessoa.Pis = vm.Pis;
            pessoa.UfIdentidade = vm.UfIdentidade;
            pessoa.Email = vm.Email;
            pessoa.Imagem = vm.Imagem;
            pessoa.Contatos = SalvarListaPessoaContato(pessoa, vm.Contatos);

            return pessoa;
        }

        private void ValidarCadastroPessoa(SalvarPessoaViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Informe o nome da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Cpf))
            {
                ModelState.AddModelError("CPF", "Informe o Cpf da Pessoa.");
            }
            if (vm.Sexo <= 0)
            {
                ModelState.AddModelError("Sexo", "Informe o Sexo da Pessoa.");
            }
            if (vm.DataNascimento == null || vm.DataNascimento.Date >= DateTime.Today)
            {
                ModelState.AddModelError("Data de Nascimento", "Informe a Data de Nascimento da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.NumeroIdentidade))
            {
                ModelState.AddModelError("RG", "Informe o RG da Pessoa.");
            }
            if (vm.OrgaoExpedidorIdentidade <= 0)
            {
                ModelState.AddModelError("RG", "Informe o Orgão Expedidor do RG da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.UfIdentidade))
            {
                ModelState.AddModelError("RG", "Informe a UF do RG da Pessoa.");
            }
            if (vm.Escolaridade <= 0)
            {
                ModelState.AddModelError("Escolaridade", "Informe a Escolaridade da Pessoa.");
            }
            if (vm.EntidadeId <= 0)
            {
                ModelState.AddModelError("Entidade", "Informe a Entidade da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Logradouro))
            {
                ModelState.AddModelError("Logradouro", "Informe o logradouro do endereço da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Bairro))
            {
                ModelState.AddModelError("Bairro", "Informe o bairro do endereço da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Cep))
            {
                ModelState.AddModelError("Cep", "Informe o CEP do endereço da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Uf))
            {
                ModelState.AddModelError("UF", "Informe a UF da Pessoa.");
            }
            if (vm.Cidade <= 0)
            {
                ModelState.AddModelError("Cidade", "Informe a cidade do endereço da Pessoa.");
            }
            if (string.IsNullOrWhiteSpace(vm.Email))
            {
                ModelState.AddModelError("Email", "Informe o Email da Pessoa.");
            }
            if (vm.Contatos.Count <= 0)
            {
                ModelState.AddModelError("Contato", "Informe pelo menos um contato válido para Pessoa.");
            }
            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        public List<EscolaridadeViewModel> RecuperarTiposEscolaridade()
        {
            List<EscolaridadeViewModel> escolaridades;
            escolaridades = _mapper.MapearEscolaridadesViewModel(_pessoaService.RecuperarEscolaridadeAll());
            return escolaridades;
        }

        public List<DeficienciaViewModel> RecuperarTiposDeficiencia()
        {
            List<DeficienciaViewModel> escolaridades;
            escolaridades = _mapper.MapearDeficienciasViewModel(_pessoaService.RecuperarDeficienciaAll());
            return escolaridades;
        }

        public List<OrgaoExpedidorViewModel> RecuperarTiposOrgaoExpedidor()
        {
            List<OrgaoExpedidorViewModel> orgaoExpedidores;
            orgaoExpedidores = _mapper.MapearOrgaosExpedidoresViewModel(_pessoaService.RecuperarOrgaoExpedidorAll());
            return orgaoExpedidores;
        }

        public List<TipoPessoaContato> RecuperarTipoContatoAll()
        {
            List<TipoPessoaContato> tipos = _pessoaService.RecuperarTipoPessoaContatoAll();
            return tipos;
        }

        public List<Sexo> RecuperarSexos()
        {
            List<Sexo> sexos = _pessoaService.RecuperarSexo();
            return sexos;
        }

        private List<PessoaContato> SalvarListaPessoaContato(Pessoa pessoa, List<PessoaContatoViewModel> contatos)
        {

            List<PessoaContato> listaContatos = new List<PessoaContato>();

            foreach (var contato in contatos)
            {
                PessoaContato contatoAux;
                if (contato.Id.HasValue && contato.Id.Value > 0)
                {
                    contatoAux = RecuperarPessoaContato(contato.Id.Value) ?? new PessoaContato();
                    //    // não tem edição na tela, o usuario exclui e cria denovo, aqui é só oq sera mantido, então não precisa alterar essas datas
                    //    //contatoAux.LastModifierUserId = this.RecuperarUsuarioId();
                    //    //contatoAux.LastModificationTime = DateTime.Now;
                }
                else
                {
                    contatoAux = new PessoaContato
                    {
                        Pessoa = pessoa,
                        CreatorUserId = this.RecuperarUsuarioId(),
                        CreationTime = DateTime.Now
                    };
                }

                contatoAux.Numero = contato.Numero;
                contatoAux.TipoPessoaContatoId = contato.TipoContatoId;
                listaContatos.Add(contatoAux);

                foreach (var item in pessoa.Contatos.Where(a => !listaContatos.Any(b => b.Id == a.Id)).ToList())
                {
                    contatoAux = new PessoaContato();
                    contatoAux = RecuperarPessoaContato(item.Id);
                    contatoAux.DeletionTime = DateTime.Now;
                    contatoAux.DeletionUserId = this.RecuperarUsuarioId();
                    listaContatos.Add(contatoAux);
                }
            }

            return listaContatos;


            //List<PessoaContato> listaContatos = new List<PessoaContato>();
            //PessoaContato contatoAux;
            //foreach(var contato in contatos)
            //{
            //	if(!pessoa.Contatos.Any(x => x.Id == contato.Id))
            //	{
            //		listaContatos.Add(new PessoaContato {
            //			Pessoa = pessoa,
            //			Numero = contato.Numero,
            //			TipoContato = RecuperarTipoPessoaContato(contato.Id)
            //		});
            //	} else
            //	{
            //		contatoAux = RecuperarPessoaContato(contato.Id);
            //		listaContatos.Add(new PessoaContato() {
            //			TipoContato = contatoAux.TipoContato,
            //			Numero = contatoAux.Numero
            //		});
            //	}
            //}
            //return listaContatos;
        }

        //private TipoPessoaContato RecuperarTipoPessoaContato(long tipoPessoaContatoId)
        //{
        //    TipoPessoaContato tipoPessoaContato = _pessoaService.RecuperarTipoPessoaContato(tipoPessoaContatoId);
        //    return tipoPessoaContato;
        //}

        private PessoaContato RecuperarPessoaContato(long idPessoaCantato)
        {
            PessoaContato pessoaContato = _pessoaService.RecuperarPessoaContato(idPessoaCantato);
            return pessoaContato;
        }
    }
}