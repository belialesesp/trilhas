using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Helper.Contract;

namespace Trilhas.Helper
{
    public  class FileHelper
    {
        private readonly IHostEnvironment _env;

        public FileHelper(IHostEnvironment env)
        {
            _env = env;
        }

        public  string GetAppDataPath()
        {
            var path = string.Concat(Path.Combine(_env.ContentRootPath, "App_Data\\Files"), "\\");
            return path;
        }

        public DownloadFileContract ObterBytesDoArquivoParaDownload(string filePathTemp)
        {
            var downloadFile = new DownloadFileContract();
            try
            {

                downloadFile.FileByte = File.ReadAllBytes(filePathTemp);
                downloadFile.FileString =  Convert.ToBase64String(downloadFile.FileByte);
                downloadFile.FileName = "RelatorioCapacitadosPorPerido.xlsx";
                downloadFile.FilePathTemp = filePathTemp;
                
                ExcluirArquivo(filePathTemp);

            }
            catch (Exception ex)
            {
                ExcluirArquivo(filePathTemp);
                throw new Exception(string.Concat("Erro ao abrir o arquivo excel temp gerado! - ", ex.Message));
            }
            return downloadFile;
        }


        public void ExcluirArquivo(string filePathTemp)
        {
            if (File.Exists(filePathTemp))
                File.Delete(filePathTemp);
        }

    }

}
