using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GiveItA.Rest
{
    public class RestResponse<T> : RestResponse where T : new()
    {

        private T _Item;

        public RestResponse(HttpWebResponse response)
            : base(response)
        {
    
        }

        
        public T Data
        {
            get
            {
                if(_Item == null)
                {
                    _Item = JsonConvert.DeserializeObject<T>(base.Content);
                }
                return _Item;
            }
        }


    }


    public class RestResponse :IDisposable
    {
        protected HttpWebResponse _response;
        protected string _content;

        public RestResponse(HttpWebResponse response)
        {
            _response = response;         
            
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
                if (_content == null)
                {
                    StreamReader responseReader = new StreamReader(_response.GetResponseStream());
                    _content = responseReader.ReadToEnd();
                    responseReader.Close();
                }
                return _content;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }


        public void Dispose()
        {
            _response.Close();
            _response = null;
        }
    }
}
