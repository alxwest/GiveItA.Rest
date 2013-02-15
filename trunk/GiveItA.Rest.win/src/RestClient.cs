using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GiveItA.Rest
{
    public class RestClient
    {
        private HttpBasicCredentials _authenticator;
        private string _baseUrl;

        public RestClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public HttpBasicCredentials Authenticator
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
            var r = BuildRequest(request);
            var response = r.GetResponse();
            
            return new RestResponse(response);
        }

        private WebRequest BuildRequest(RestRequest restRequest)
        {
            var request = WebRequest.Create(restRequest.GetUri(_baseUrl));
            if(_authenticator!=null)
            {
                restRequest.SetBasicAuth(_authenticator.Username, _authenticator.Password);
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
    }
}
