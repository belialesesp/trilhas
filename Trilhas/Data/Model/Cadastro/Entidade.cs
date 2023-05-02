using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Trilhas.Data.Model.Cadastro
{
    public class Entidade : DefaultEntity
    {
        public string Nome { get; set; }

        public string Sigla { get; set; }

        [ForeignKey("TipoId")]
        public TipoDeEntidade Tipo { get; set; }
        public long TipoId { get; set; }

        public Municipio Municipio { get; set; }

        public List<Gestor> Gestores { get; set; }

        public bool AdicionarGestor(Pessoa pessoa)
        {
            if (Gestores == null)
            {
                Gestores = new List<Gestor>();
            }

            if (!EhGestor(pessoa))
            {
                Gestores.Add(new Gestor
                {
                    Pessoa = pessoa,
                    Entidade = this
                });

                return true;
            }

            return false;
        }

        public bool RemoverGestor(long pessoaId)
        {
            var gestor = Gestores.FirstOrDefault(x => x.Pessoa.Id == pessoaId);

            if (gestor != null)
            {
                Gestores.Remove(gestor);
                return true;
            }

            return false;
        }

        public bool EhGestor(Pessoa pessoa)
        {
            return Gestores.Any(x => x.Pessoa.Id == pessoa.Id);
        }
    }
}
