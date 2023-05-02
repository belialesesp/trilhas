using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Trilhas.Data;
using Trilhas.Settings;

namespace Trilhas.Services
{
    public class MinioService
    {
        private MinioSettings minioSettings;
        private MinioClient minio;

        private DateTime horaUltimoErro;
        private string ultimoErro;

        public MinioService(IOptions<MinioSettings> minioOptions)
        {
            try
            {
                this.minioSettings = minioOptions.Value;

                this.minio = new MinioClient(minioSettings.Server, minioSettings.AccessKey, minioSettings.SecretKey);

                if (minioSettings.UseSSL)
                {
                    this.minio = this.minio.WithSSL();
                }
            }
            catch (Exception ex)
            {
                RegistrarErro(ex);
            }
        }

        public List<Item> ListarArquivosCkeditor()
        {
            List<Item> arquivos = new List<Item>();

            try
            {
                IObservable<Item> observable = minio.ListObjectsAsync(minioSettings.CkeditorBucket);

                IDisposable subscription = observable.Subscribe(
                    item => arquivos.Add(item),
                    ex => throw ex);

                observable.Wait();
                subscription.Dispose();
            }
            catch (Exception ex)
            {
                RegistrarErro(ex);
            }

            return arquivos;
        }

        public void SalvarImagemEixo(Arquivo arquivo)
        {
            Salvar(arquivo, minioSettings.EixosBucket);
        }

        public void SalvarImagemPessoa(Arquivo arquivo)
        {
            Salvar(arquivo, minioSettings.PessoasBucket);
        }

        private void Salvar(Arquivo arquivo, String bucket)
        {
            try
            {
                VerificarBucket(bucket);

                Stream fileStream = arquivo.ArquivoStream;

                minio.PutObjectAsync(bucket, arquivo.Nome, fileStream, fileStream.Length, "application/octet-stream");
            }
            catch (Exception ex)
            {
                RegistrarErro(ex);
            }
        }

        public async Task<Arquivo> RecuperarImagemEixo(string nome)
        {
            return await RecuperarImagem(nome, minioSettings.EixosBucket);
        }

        public async Task<Arquivo> RecuperarImagemPessoa(string nome)
        {
            return await RecuperarImagem(nome, minioSettings.PessoasBucket);
        }

        private async Task<Arquivo> RecuperarImagem(string nome, string bucket)
        {
            try
            {
                MemoryStream ms = null;

                await minio.GetObjectAsync(bucket, nome, (stream) =>
                {
                    ms = new MemoryStream();
                    stream.CopyTo(ms);
                });

                var arquivo = new Arquivo { Nome = nome };
                arquivo.FromMemoryStream(ms);

                return arquivo;
            }
            catch (ObjectNotFoundException nfex)
            {
                return new Arquivo
                {
                    Nome = null,
                    ArquivoBase64 = ""
                };
            }
            catch (Exception ex)
            {
                RegistrarErro(ex);
                return null;
            }
        }

        public void ExcluirImagemEixo(string nome)
        {
            ExcluirImagem(nome, minioSettings.EixosBucket);
        }

        public void ExcluirImagemPessoa(string nome)
        {
            ExcluirImagem(nome, minioSettings.PessoasBucket);
        }

        private void ExcluirImagem(string nome, string bucket)
        {
            try
            {
                this.minio.RemoveObjectAsync(bucket, nome);
            }
            catch (Exception ex)
            {
                RegistrarErro(ex);
            }
        }

        private void VerificarBucket(string bucketName)
        {
            bool found = minio.BucketExistsAsync(bucketName).Result;

            if (!found)
            {
                minio.MakeBucketAsync(bucketName);
            }
        }

        private void RegistrarErro(Exception ex)
        {
            this.horaUltimoErro = DateTime.Now;
            this.ultimoErro = ex.Message;
        }

        public MinioSettings Settings()
        {
            return minioSettings;
        }

        public string UltimoErro()
        {
            return this.ultimoErro;
        }

        public DateTime HoraUltimoErro()
        {
            return this.horaUltimoErro;
        }
    }
}
