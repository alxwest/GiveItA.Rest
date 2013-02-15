using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GiveItA.Rest
{
    public class RestParameter
    {
        private string _Key;
        private string _Value;
        private ParamType _ParamType;

        public RestParameter(string key, string value, ParamType paramType)
        {
            _Key = key;
            _Value = value;
            _ParamType = paramType;
        }

        public string Key
        {
            get
            {
                return _Key;
            }
        }

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public string GetValueEncoded()
        {
            return HttpUtility.HtmlEncode(_Value);
        }

        public ParamType ParamType
        {
            get
            {
                return _ParamType;
            }
        }

    }
}
