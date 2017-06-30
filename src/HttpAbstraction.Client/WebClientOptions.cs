namespace HttpAbstraction.Client
{
    public class WebClientOptions
    {
        public string BaseUri { get; set; }
        public int TimeoutRetries { get; set; }
        public int ConnectionLimit { get; set; }
        public int ConnectionTimeoutSeconds { get; set; }

        public WebClientOptions()
        {
            ConnectionLimit = 10;
            ConnectionTimeoutSeconds = 30;
        }

    }
}
