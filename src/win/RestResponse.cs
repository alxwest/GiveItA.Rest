using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GiveItA.Rest
{
    public class RestResponse
    {
        private WebResponse _response;
        private string _content;

        public RestResponse(WebResponse response)
        {
            _response = response;
            //read response
            StreamReader responseReader = new StreamReader(_response.GetResponseStream());
            _content = responseReader.ReadToEnd();
            responseReader.Close();
            response.Close();
        }

        public byte[] RawBytes
        {
            get
            {
                return Encoding.UTF8.GetBytes(_content);
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
        }
    }
}
