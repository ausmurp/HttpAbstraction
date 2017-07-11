using System.Collections.Generic;

namespace HttpAbstraction.Client
{
    public class WebClientOptions
    {
        public string BaseUri { get; set; }
        public int RetryAttempts { get; set; }
        public int RetryDelaySeconds { get; set; }
        public int ConnectionTimeoutSeconds { get; set; }

        public WebClientOptions()
        {
            ConnectionTimeoutSeconds = 30;
        }

    }
}
