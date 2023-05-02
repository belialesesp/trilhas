namespace Trilhas.Settings
{
    public class MinioSettings
    {
        public string Server { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool UseSSL { get; set; }
        public string EixosBucket { get; set; }
        public string PessoasBucket { get; set; }
        public string CkeditorBucket { get; set; }
    }
}
