using System;
using System.Collections.Generic;
using System.Text;

namespace HttpAbstraction.Client
{
    public class RetryClientOptions : WebClientOptions
    {
        public int RetryAttempts { get; set; }
        public int RetryDelaySeconds { get; set; }

    }
}
