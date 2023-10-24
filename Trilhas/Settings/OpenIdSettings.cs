namespace Trilhas.Settings
{
    public class OpenIdSettings
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool SaveTokens { get; set; }
        public string CallbackPath { get; set; }
    }
}
