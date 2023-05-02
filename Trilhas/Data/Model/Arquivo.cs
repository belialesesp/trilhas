using System;
using System.IO;

namespace Trilhas.Data
{
    public class Arquivo
    {
        public string Nome { get; set; }
        public string ArquivoBase64 { get; set; }
        public MemoryStream ArquivoStream { get; set; }

        public void FromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return;
            }

            ArquivoBase64 = base64;
            byte[] bytes = Convert.FromBase64String(ArquivoBase64);
            ArquivoStream = new MemoryStream(bytes);
        }

        public void FromMemoryStream(MemoryStream ms)
        {
            if (ms == null)
            {
                return;
            }

            ArquivoStream = ms;
            ArquivoBase64 = Convert.ToBase64String(ArquivoStream.ToArray());
        }
    }
}
