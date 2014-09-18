using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveItA.Rest.Auth
{
    public class OAuth2Credentials : ICredentials
    {
        public OAuth2Credentials(string clientId, string clientSecret, string oAuthServerURL)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            OAuthServerURL = oAuthServerURL;
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string OAuthServerURL { get; set; }
    }
}
