namespace Trilhas.Data.Model.Cadastro
{
    public class Municipio : Entity
    {
        public long codigoMunicipio { get; set; }
        public string NomeMunicipio { get; set; }
        public int codigoUf { get; set; }
        public string Uf { get; set; }
    }
}
