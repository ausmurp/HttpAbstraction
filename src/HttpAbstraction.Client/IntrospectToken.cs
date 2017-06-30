using System;
using System.Runtime.Serialization;

namespace HttpAbstraction.Client
{
    [DataContract]
    public class IntrospectToken
    {
        private int? _lifetimeSeconds;

        [DataMember(Name = "active")]
        public bool IsActive { get; set; }

        [DataMember(Name = "token_type")]
        public string Type { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "exp")]
        public int? LifetimeSeconds
        {
            get
            {
                return _lifetimeSeconds;
            }
            set
            {
                if (value.HasValue)
                {
                    var date = new DateTime(1970, 1, 1);
                    date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                    date = date.AddSeconds(value.Value);

                    _lifetimeSeconds = Convert.ToInt32((date - DateTime.UtcNow).TotalSeconds);
                }
                else
                {
                    _lifetimeSeconds = null;
                }


            }
        }
    }
}
