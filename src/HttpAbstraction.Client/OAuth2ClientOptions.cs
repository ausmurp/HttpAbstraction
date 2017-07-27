namespace HttpAbstraction.Client
{
    public class OAuth2ClientOptions<TGrant> : RetryClientOptions
    {
        public string TokenPath { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool HasIntrospection { get; set; }
        public TGrant GrantOptions { get; set; }

        public OAuth2ClientOptions(TGrant grantOptions)
        {
            GrantOptions = grantOptions;
        }
    }
}
