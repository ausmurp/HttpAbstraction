using System.Net.Http;

namespace HttpAbstraction.Client
{
    public class OAuth2Client<TGrant> : RetryClient
    {
        public OAuth2Client(OAuth2ClientOptions<TGrant> options, HttpMessageHandler handler = null, bool disposeHandler = true) : base(options, new OAuth2Handler<TGrant>(options, handler), disposeHandler)
        {
        }

    }
}
