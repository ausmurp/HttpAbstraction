using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpAbstraction.Client
{
    public class ConnectionHandler: WinHttpHandler
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
}
