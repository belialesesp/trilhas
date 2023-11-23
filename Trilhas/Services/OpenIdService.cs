using Microsoft.Extensions.Options;
using Trilhas.Settings;

namespace Trilhas.Services
{
    public class OpenIdService
    {
        private readonly OpenIdSettings _openIdSettings;

        public OpenIdService(IOptions<OpenIdSettings> settings)
        {
            _openIdSettings = settings.Value;
        }

        public string Authority
        {
            get { return _openIdSettings?.Authority; }
        }

        public string ClientId
        {
            get { return _openIdSettings?.ClientId; }
        }

        public string ClientSecret
        {
            get { return _openIdSettings?.ClientSecret; }
        }

        public bool SaveTokens
        {
            get { return _openIdSettings.SaveTokens; }
        }

        public string CallbackPath
        {
            get { return _openIdSettings?.CallbackPath; }
        }
    }
}
