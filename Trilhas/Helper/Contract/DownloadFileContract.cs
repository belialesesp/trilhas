namespace Trilhas.Helper.Contract
{
    public class DownloadFileContract
    {
        public string FileName { get; set; }

        public byte[] FileByte { get; set; }

        public string FileString { get; set; }

        public string FilePathTemp { get; set; }


        public DownloadFileContract() { }

        public DownloadFileContract(DownloadFileContract e)
        {
            this.FileName = e.FileName;
            this.FileByte = e.FileByte;
            this.FilePathTemp = e.FilePathTemp;
        }

    }
}
