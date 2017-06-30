using System.Runtime.Serialization;

namespace HttpAbstraction.Client
{
    [DataContract]
    public class ResourceOwnerGrantOptions
    {
        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "grant_type")]
        public string GrantType { get { return "password"; } }
        

    }
}
