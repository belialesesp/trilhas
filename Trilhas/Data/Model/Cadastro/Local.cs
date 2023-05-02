using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Trilhas.Data.Model.Cadastro
{
    public class Local : DefaultEntity
    {
        public string Nome { get; set; }
        public string Observacoes { get; set; }

        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        [ForeignKey("MunicipioId")]
        public Municipio Municipio { get; set; }

        public List<LocalContato> Contatos { get; set; }
        public List<LocalRecurso> Recursos { get; set; }
        public List<LocalSala> Salas { get; set; }

        public int CapacidadeTotal()
        {
            int capacidadeTotal = 0;

            foreach (var sala in Salas.Where(x => !x.DeletionTime.HasValue))
            {
                capacidadeTotal += sala.Capacidade;
            }

            return capacidadeTotal;
        }

        public void AdicionarSala()
        {

        }
    }
}
