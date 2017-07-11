using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpAbstraction.Client
{
    public class WebClient : HttpClient
    {
        public WebClient(WebClientOptions options, HttpMessageHandler handler = null, bool disposeHandler = true) : base(new RetryHandler(options.RetryAttempts, options.RetryDelaySeconds, handler), disposeHandler)
        {
            BaseAddress = new Uri(options.BaseUri);
            Timeout = TimeSpan.FromSeconds(options.ConnectionTimeoutSeconds);
        }

        public async Task<TResult> Get<TResult>(string uri, Dictionary<string, string> queryParams = null)
        {
            TResult result;

            using (var request = GetRequest(HttpMethod.Get, uri, queryParams))
            {
                using (var response = await SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result = await GetResult<TResult>(response);
                }
            }

            return result;
        }

        public async Task<TResult> Put<TResult>(string uri, object item, Dictionary<string, string> queryParams = null)
        {
            TResult result;

            using (var request = GetRequest(HttpMethod.Put, uri, queryParams, item))
            {
                using (var response = await SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result = await GetResult<TResult>(response);
                }
            }

            return result;
        }

        public async Task<TResult> Patch<TResult>(string uri, object item, Dictionary<string, string> queryParams = null)
        {
            TResult result;

            using (var request = GetRequest(new HttpMethod("PATCH"), uri, queryParams, item))
            {
                using (var response = await SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result = await GetResult<TResult>(response);
                }
            }

            return result;
        }

        public async Task<TResult> Post<TResult>(string uri, object item, Dictionary<string, string> queryParams = null)
        {
            TResult result;

            using (var request = GetRequest(HttpMethod.Post, uri, queryParams, item))
            {
                using (var response = await SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result = await GetResult<TResult>(response);
                }
            }

            return result;
        }

        public async Task<TResult> Delete<TResult>(string uri, Dictionary<string, string> queryParams = null)
        {
            TResult result;

            using (var request = GetRequest(HttpMethod.Delete, uri, queryParams))
            {
                using (var response = await SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result = await GetResult<TResult>(response);
                }
            }

            return result;
        }

        protected HttpRequestMessage GetRequest(HttpMethod httpMethod, string uri, Dictionary<string, string> queryParams = null, object content = null)
        {
            if (queryParams != null && queryParams.Count > 0)
                uri += GetQueryString(queryParams, uri.Contains("?"));

            var request = new HttpRequestMessage(httpMethod, uri);

            if (content != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            return request;
        }

        protected string GetQueryString(Dictionary<string, string> queryParams, bool alreadyHasParams = false)
        {
            string queryString = alreadyHasParams ? "&" : "?";

            List<string> queryList = new List<string>(queryParams.Count);
            queryParams.ToList().ForEach(p => queryList.Add($"{p.Key}={p.Value}"));
            queryString += string.Join("&", queryList);

            return queryString;
        }

        protected async Task<TResult> GetResult<TResult>(HttpResponseMessage response, bool isResultCamelCase = true)
        {
            var json = await response.Content.ReadAsStringAsync();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            if (isResultCamelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var result = JsonConvert.DeserializeObject<TResult>(json, settings);

            return result;
        }


    }
}
