using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpAbstraction.Client
{
    public class WebHandler: DelegatingHandler
    {
        private readonly WebClientOptions _options;

        public WebHandler(WebClientOptions options, HttpMessageHandler innerHandler = null)
        {
            _options = options;
            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode && _options.TimeoutRetries > 0)
                {
                    for (int attempt = 0; attempt < _options.TimeoutRetries; attempt++)
                    {
                        response = await base.SendAsync(request, cancellationToken);

                        if (response.IsSuccessStatusCode)
                            break;
                    }
                }

                return response;
            }
            catch (TaskCanceledException ex) //HttpClient throws this on timeout
            {
                //we need to convert it to a different exception
                //otherwise ExecuteAsync will think we requested cancellation
                throw new HttpRequestException("Request timed out", ex);
            }
        }
    }
}
