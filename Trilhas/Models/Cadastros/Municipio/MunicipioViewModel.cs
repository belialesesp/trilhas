namespace Trilhas.Models.Cadastros.Municipio
{
    public class MunicipioViewModel
	{
		public MunicipioViewModel(long id){
			Id = id;
		}
		public long Id { get; set; }
		public long codigoMunicipio { get; set; }
		public string NomeMunicipio { get; set; }
		public int codigoUf { get; set; }
		public string Uf { get; set; }
	}
}
