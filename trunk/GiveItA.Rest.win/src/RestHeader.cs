using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace GiveItA.Rest
{
    public class RestHeader
    {
        private readonly ObservableCollection<RestParameter> _Headers;

        public struct Consts
        {
            //public const string Content_Length = "Content-Length";
            //public const string Cookie = "Cookie";
            public const string Authorization = "Authorization";
            public const string Accept = "Accept";
            public const string Accept_Charset = "Accept-Charset";
            public const string Accept_Encoding = "Accept-Encoding";
            public const string Accept_Language = "Accept-Language";
            public const string User_Agent = "User-Agent"; 
        }

        

        public RestHeader()
        {
            _Headers = new ObservableCollection<RestParameter>();
        }

        public ICollection<RestParameter> Headers
        {
            get
            {
                return _Headers;
            }
        }

        public IDictionary<string, string> GetRawHeaders()
        {
            return _Headers.ToDictionary(x => x.Key, x => x.GetValueEncoded());
        }
        
    }
}
