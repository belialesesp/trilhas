using System;
using System.IO;
using System.Text;

namespace Trilhas.Data
{
    public class Arquivo
    {
        public string Nome { get; set; }
        public string ArquivoBase64 { get; set; }
        public MemoryStream ArquivoStream { get; set; }

        public string GetBase64()
        {
            if (ArquivoStream == null)
                return string.Empty;

            FromMemoryStream(ArquivoStream);

            return ArquivoBase64;
        }

        public void FromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return;

            ArquivoBase64 = base64;
            byte[] bytes = Convert.FromBase64String(ArquivoBase64);
            ArquivoStream = new MemoryStream(bytes);
        }

        public void FromMemoryStream(MemoryStream ms)
        {
            if (ms == null)
                return;

            ArquivoStream = ms;
            ArquivoBase64 = Convert.ToBase64String(ArquivoStream.ToArray());
        }

        public void FromString(string html)
        {
            if (string.IsNullOrEmpty(html))
                return;

            byte[] bytes = Encoding.UTF8.GetBytes(html);
            MemoryStream ms = new MemoryStream(bytes);

            ArquivoStream = ms;
        }
    }
}
