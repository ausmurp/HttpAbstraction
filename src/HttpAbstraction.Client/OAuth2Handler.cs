using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace HttpAbstraction.Client
{
    public class OAuth2Handler<TGrant> : DelegatingHandler
    {
        private readonly object _lockObj;
        private readonly OAuth2ClientOptions<TGrant> _options;
        private readonly HttpClient _authClient;
        private Token _token;

        public OAuth2Handler(OAuth2ClientOptions<TGrant> options, HttpMessageHandler innerHandler = null)
        {
            _options = options;

            //The idea here is that the client should be instantiated on the main thread only when used inside 
            //a threaded process. This will ensure that the Authorize() method is called only once.
            _lockObj = new object();

            _authClient = innerHandler == null ? new HttpClient() : new HttpClient(innerHandler, true);
            _authClient.BaseAddress = new Uri($"{options.BaseUri}/{options.TokenPath}/".Replace(@"//", @"/").Replace(@":/", @"://"));

            string secret = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.ClientId}:{System.Net.WebUtility.UrlEncode(options.ClientSecret)}"));
            _authClient.DefaultRequestHeaders.Add("Authorization", $"Basic {secret}");

            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_token == null || _token.IsExpired)
                _token = GetToken();

            SetAuthHeader(request, _token);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            return response;
        }

        /// <summary>
        /// Locks to ensure only a single thread can authorize. This ensures the other threads use the token returned by the first.
        /// </summary>
        /// <returns></returns>
        protected Token GetToken()
        {
            lock (_lockObj)
            {
                if (_token == null || _token.IsExpired)
                {
                    _token = Authorize().Result;
                }
            }

            return _token;
        }

        protected async Task<Token> Authorize()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "");

            //serialize options to dictionary
            var requestParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(_options.GrantOptions, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            request.Content = new FormUrlEncodedContent(requestParams);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _authClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            //Set token and type
            var token = JsonConvert.DeserializeObject<Token>(content);
            token.LifetimeStart = DateTime.Now;

            //Introspect to get more detail about the token like scope, lifetime, etc.
            //Do not auto refresh as that could cause infinite loop which would in turn be a timeout
            if (_options.HasIntrospection)
                token = await Introspect(token, false);

            return token;
        }

        protected async Task<Token> Introspect(Token token, bool autoRefresh = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "introspection");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "token", token.AccessToken } });
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _authClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            var introspectToken = JsonConvert.DeserializeObject<IntrospectToken>(content);

            if (autoRefresh && !introspectToken.IsActive)
            {
                //Get new token
                token = await Authorize();
            }
            else
            {
                if (introspectToken.LifetimeSeconds.HasValue)
                    token.LifetimeSeconds = introspectToken.LifetimeSeconds;

                if (!String.IsNullOrWhiteSpace(introspectToken.Scope))
                    token.Scope =  introspectToken.Scope;
            }

            return token;
        }

        protected void SetAuthHeader(HttpRequestMessage request, Token token)
        {
            if (request.Headers.Contains("Authorization"))
                request.Headers.Remove("Authorization");

            request.Headers.Add("Authorization", $"{token.Type} {token.AccessToken}");
        }

        protected override void Dispose(bool disposing)
        {
            //Disposing authClient auto disposes innerHandler
            if (disposing)
            {
                _authClient?.Dispose();
            }

            base.Dispose(disposing);
        }


    }
}
