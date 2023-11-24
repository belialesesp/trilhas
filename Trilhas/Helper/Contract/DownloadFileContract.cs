namespace Trilhas.Helper.Contract
{
    public class DownloadFileContract
    {
        public string FileName { get; set; }

        public byte[] FileByte { get; set; }

        public string FileString { get; set; }

        public string FilePathTemp { get; set; }


        public DownloadFileContract() { }

    }
}
