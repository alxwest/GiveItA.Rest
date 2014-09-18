using GiveItA.Rest.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GiveItA.Rest
{
    public class RestClient : IDisposable
    {
        private BaseAuthenticator _authenticator;
        private string _baseUrl;

        public RestClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public BaseAuthenticator Authenticator
        {
            get
            {
                return _authenticator;
            }
            set
            {
                _authenticator = value;
            }
        }

        public RestResponse Execute(RestRequest request)
        {
            return new RestResponse(ExecuteRequest(request));
        }

        public RestResponse<T> Execute<T>(RestRequest request) where T : new()
        {
            return new RestResponse<T>(ExecuteRequest(request));
        }

        private HttpWebResponse ExecuteRequest(RestRequest request)
        {
            var r = BuildRequest(request);
            HttpWebResponse response = null;
            try
            {
                response = r.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;                
                if (response == null)
                    throw ex;
            }
            //if a 3xx code and redirect with Auth
            if (((int)response.StatusCode & 288) == 288 && request.FollowRedirectsWithAuth)
            {
                string location = response.Headers["location"];
                //If it starts with a slash presume it wants to be absolute
                if (location.StartsWith("/"))
                {
                    location = string.Format("~{0}", response.Headers["location"]);
                }
                //Not going across domain just yet.
                else if (location.Contains("://"))
                {
                    throw new WebException("Cross domain redirects are not supported");
                }
                request.ChangeLocation(location);
                return ExecuteRequest(request);
            }

            return response;
        }

        private HttpWebRequest BuildRequest(RestRequest restRequest)
        {
            var request = HttpWebRequest.Create(restRequest.GetUri(_baseUrl)) as HttpWebRequest;
            request.AllowAutoRedirect = restRequest.FollowRedirects &! restRequest.FollowRedirectsWithAuth;            
            if(_authenticator!=null)
            {
               _authenticator.Authenticate(restRequest); 
            }
            request.Method = restRequest.GetMethod();
            if ((restRequest.Method & Method.SAFE) == 0)
            {
                string body = restRequest.GetBody();
                var streamwriter = new StreamWriter(request.GetRequestStream());
                streamwriter.Write(body);
                streamwriter.Close();
            }
            request.ContentType = restRequest.GetContentType();
            foreach (var header in restRequest.GetHeaders())
            {
                if (header.Key != "Content-Type")
                    request.Headers[header.Key] = header.Value;
            }
            return request;
        }

        public void Dispose()
        {
            //TODO close any open responses
        }
    }
}
