using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveItA.Rest.Auth
{
    public class Authenticator :BaseAuthenticator
    {
        private OAuthToken _token;

        public Authenticator(ICredentials credentials) : base(credentials) { }

        public override void Authenticate(RestRequest request)
        {
            if (typeof(HttpBasicCredentials) == _credentials.GetType())
            {
                //set the Basic Auth header.
                var credentials = (HttpBasicCredentials)_credentials;
                var authData = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", credentials.Username, credentials.Password))));
                request.PutHeader("Authorization", authData);   
            }
            if (typeof(OAuth2Credentials) == _credentials.GetType())
            {
                var credentials = (OAuth2Credentials)_credentials;
                if (_token == null)
                {
                    //get token
                    using (var client = new RestClient(credentials.OAuthServerURL))
                    {
                        var tokenRequest = new RestRequest("", Method.POST);
                       // tokenRequest.ContentType = "application/x-www-form-urlencoded";
                        tokenRequest.AddParameter("client_id", credentials.ClientId);
                        tokenRequest.AddParameter("client_secret", credentials.ClientSecret);
                        tokenRequest.AddParameter("grant_type", "client_credentials");
                        tokenRequest.AddParameter("response_type", "token");
                        var response = client.Execute<OAuthToken>(tokenRequest);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            _token = response.Data;
                        }
                    }
                }
                //get token refresh
                else if (DateTime.UtcNow > _token.UTCExpiresOn)
                {
                    using (var client = new RestClient(credentials.OAuthServerURL))
                    {
                        var tokenRequest = new RestRequest("", Method.POST);
                        //tokenRequest.ContentType = "application/x-www-form-urlencoded";
                        tokenRequest.AddParameter("refresh_token", _token.refresh_token);
                        tokenRequest.AddParameter("grant_type", "refresh_token");
                        tokenRequest.AddParameter("response_type", "token");
                        var response = client.Execute<OAuthToken>(tokenRequest);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            _token = response.Data;
                        }
                    }
                }
                var authData = string.Format("{0} {1}", _token.token_type, _token.access_token);
                request.PutHeader("Authorization", authData);   
            }
        }
    }
}
