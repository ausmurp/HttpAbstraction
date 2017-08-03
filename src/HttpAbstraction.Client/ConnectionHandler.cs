using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpAbstraction.Client
{
#if NETSTANDARD
    using System.Net.Http;

    public class ConnectionHandler : WinHttpHandler
    {
        private readonly ConnectionOptions _options;

        public ConnectionHandler(ConnectionOptions options)
        {
            _options = options;
            this.MaxConnectionsPerServer = _options.ConnectionLimit;
            this.ReceiveDataTimeout = TimeSpan.FromSeconds(_options.ResponseTimeoutSeconds);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
#else
     using System.Net;
    using System.Net.Http;

    public class ConnectionHandler : HttpClientHandler
    {
        private readonly ConnectionOptions _options;

        public ConnectionHandler(ConnectionOptions options)
        {
            _options = options;
            var servicePoint = ServicePointManager.FindServicePoint(new Uri(options.BaseUri));
            servicePoint.ConnectionLimit = options.ConnectionLimit;
            // Set lease timeout to 1 second more than the connection timeout.
            servicePoint.ConnectionLeaseTimeout = (options.ConnectionTimeoutSeconds * 1000) + 1;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
#endif

}
