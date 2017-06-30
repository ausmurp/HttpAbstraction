using System;
using System.Runtime.Serialization;

namespace HttpAbstraction.Client
{
    [DataContract]
    public class Token
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string Type { get; set; }

        [DataMember(Name = "expires_in")]
        public int? LifetimeSeconds { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        public DateTime? LifetimeStart { get; set; }

        public bool IsExpired
        {
            get
            {
                bool isExp = LifetimeStart.HasValue && LifetimeSeconds.HasValue && LifetimeStart.Value.AddSeconds(LifetimeSeconds.Value) < DateTime.Now;

                return isExp;
            }
        }

    }
}
