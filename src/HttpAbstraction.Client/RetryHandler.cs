using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpAbstraction.Client
{
    public class RetryHandler : DelegatingHandler
    {
        private readonly RetryClientOptions _options;

        public RetryHandler(RetryClientOptions options, HttpMessageHandler innerHandler = null) : base(
            innerHandler ?? new WinHttpHandler())
        {
            _options = options;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode && _options.RetryAttempts > 0)
                {
                    int attempts = 0;

                    while (attempts < _options.RetryAttempts)
                    {
                        try
                        {
                            // base.SendAsync calls the inner handler
                            response = await base.SendAsync(request, cancellationToken);
                            attempts++;

                            if (response.IsSuccessStatusCode)
                                break;

                            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                            {
                                // 503 Service Unavailable
                                // Wait a bit and try again later
                                await Task.Delay(_options.RetryDelaySeconds, cancellationToken);
                            }
                            else if (response.StatusCode == (HttpStatusCode)429)
                            {
                                // 429 Too many requests
                                // Wait a bit and try again later
                                await Task.Delay(_options.RetryDelaySeconds, cancellationToken);
                            }

                            // Not something we can retry so return the response as is
                        }
                        catch (Exception ex) when (IsNetworkError(ex))
                        {
                            // Network error
                            // Wait a bit and try again later
                            await Task.Delay(_options.RetryDelaySeconds, cancellationToken);
                        }
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

        private static bool IsNetworkError(Exception ex)
        {
            // Check if it's a network error
            if (ex is System.Net.Sockets.SocketException)
                return true;
            if (ex.InnerException != null)
                return IsNetworkError(ex.InnerException);

            return false;
        }

    }

}
