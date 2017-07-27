using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HttpAbstraction.Client
{
    public class RetryClient : WebClient
    {
        public RetryClient(RetryClientOptions options, HttpMessageHandler handler = null, bool disposeHandler = true) : base(options, new RetryHandler(options, handler), disposeHandler)
        {
        }
    }
}
