namespace HttpAbstraction.Client
{
    public class ConnectionOptions : WebClientOptions
    {
        public int ConnectionLimit { get; set; }
        public int ResponseTimeoutSeconds { get; set; }
    }
}
