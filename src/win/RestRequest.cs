using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace GiveItA.Rest
{
    public class RestRequest
    {
        private readonly string _uri;
        private readonly Method _method;
        private DataFormat _dataFormat;
        private readonly Encoding _encoding;
        private readonly IList<RestParameter> _parameters;
        private readonly IList<Object> _body;


        public RestRequest(string uri, Method method)
            : this(uri, method, Encoding.UTF8)
        { }

        public RestRequest(string uri, Method method, Encoding encoding)
        {
            _uri = uri;
            _method = method;
            _parameters = new List<RestParameter>();
            _body = new List<object>();
            _encoding = encoding;
        }

        public void AddParameter(string key, string value)
        {
            _parameters.Add(new RestParameter(key, value, ParamType.Form));
        }

        public void AddQuery(string key, string value)
        {
            _parameters.Add(new RestParameter(key, value, ParamType.Query));
        }

        public void AddParameter(string key, string value, ParamType paramType)
        {
            _parameters.Add(new RestParameter(key, value, paramType));
        }

        public void AddUrlSegment(string key, string value)
        {
            _parameters.Add(new RestParameter(key, value, ParamType.Segment));
        }

        public void AddBody(object obj)
        {
           _body.Add(obj);
        }

        public string ContentType
        {
            get
            {
                return GetContentType();
            }
            set
            {
                SetContentType(value);
            }
        }

        public string GetContentType()
        {
            var ct = _parameters.FirstOrDefault(x => x.Key == "Content-Type");
            if (ct != null)
            {
                return ct.Value;
            }
            return null;
        }

        public void SetContentType(string contentType)
        {
            if (!_parameters.Any(x => x.Key == "Content-Type" && x.ParamType == ParamType.Header))
            {
                _parameters.Add(new RestParameter("Content-Type", contentType, ParamType.Header));
            }
            else
            {
                _parameters.First(x => x.Key == "Content-Type" && x.ParamType == ParamType.Header).Value = contentType;
            }
        }

        public void PutHeader(string key, string value)
        {
            if (!_parameters.Any(x => x.Key == key && x.ParamType == ParamType.Header))
            {
                _parameters.Add(new RestParameter(key, value, ParamType.Header));
            }
            else
            {
                _parameters.First(x => x.Key == key && x.ParamType == ParamType.Header).Value = value;
            }
        }

        public void SetBasicAuth(string userName, string userPassword)
        {
            var authData = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, userPassword))));
            PutHeader("Authorization", authData);
        }

        public Uri GetUri(string baseUri)
        {
            StringBuilder query = new StringBuilder();
            //format the URI
            if ((_method & Method.SAFE) != 0)
            {
                var p = _parameters.Where(x => (x.ParamType & ParamType.Form) != 0).ToList();
                for (int i = 0; i < p.Count; i++)
                {
                    var param = p[i];
                    query.AppendFormat("{2}{0}={1}", param.Key, param.GetValueEncoded(), i == 0 && query.Length == 0 ? "" : "&");
                }
            }
            else
            {
                var p = _parameters.Where(x => x.ParamType == ParamType.Query).ToList();
                for (int i = 0; i < p.Count; i++)
                {
                    var param = p[i];
                    query.AppendFormat("{2}{0}={1}", param.Key, param.GetValueEncoded(), i == 0 && query.Length == 0 ? "" : "&");
                }
            }
            string u = _uri;
            foreach (var segment in _parameters.Where(x => x.ParamType == ParamType.Segment))
            {
                u = u.Replace("{" + segment.Key + "}", segment.GetValueEncoded());
            }
            UriBuilder uri = new UriBuilder(new Uri(new Uri(baseUri), u));
            uri.Query = query.ToString();

            return uri.Uri;
        }

        public Method Method
        {
            get
            {
                return _method;
            }
        }

        public string GetMethod()
        {
            switch (_method)
            {
                case Method.POST:
                    return "POST";
                case Method.HEAD:
                    return "HEAD";
                case Method.GET:
                    return "GET";
                case Method.PUT:
                    return "PUT";
                case Method.DELETE:
                    return "DELETE";
                default:
                    throw new Exception("Method not accepted");
            }
        }

        public DataFormat DataFormat
        {
            get
            {
                return GetDataFormat();
            }
            set
            {
                SetDataFormat(value);
            }
        }

        public DataFormat GetDataFormat()
        {
            return _dataFormat;
        }

        public void SetDataFormat(DataFormat dataFormat)
        {
            _dataFormat = dataFormat;
        }

        public IDictionary<string, string> GetHeaders()
        {
            return _parameters.Where(x => x.ParamType == ParamType.Header).ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetBody()
        {
            StringBuilder body = new StringBuilder();

            //It's not a safe method
            if ((_method & Method.SAFE) == 0)
            {
                if (_body.Any())
                {
                    using (var stringWriter = new StringWriterWithEncoding(body, _encoding))
                        if (_dataFormat == DataFormat.Xml)
                        {
                            using (var xmlWriter = new XmlTextWriter(stringWriter))
                            {
                                foreach (var obj in _body)
                                {
                                    var x = new XmlSerializer(obj.GetType());
                                    x.Serialize(xmlWriter, obj);
                                }
                            }
                            SetContentType("application/xml");
                        }
                        else
                        {
                            using (var jsonWriter = new JsonTextWriter(stringWriter))
                            {
                                foreach (var obj in _body)
                                {
                                    var j = new JsonSerializer();
                                    j.Serialize(jsonWriter, obj);
                                }
                            }
                            SetContentType("application/json");
                        }
                }
                else
                {
                    var p = _parameters.Where(x => x.ParamType == ParamType.Form || x.ParamType == ParamType.Body).ToList();
                    for (int i = 0; i < p.Count; i++)
                    {
                        var param = p[i];
                        body.AppendFormat("{2}{0}={1}", param.Key, param.GetValueEncoded(), i == 0 && body.Length < 1 ? "" : "&");
                    }
                    if (p.Any())
                    {
                        SetContentType("application/x-www-form-urlencoded");
                    }
                }
            }
            else
            {
                if (_parameters.Any(x => (x.ParamType & ParamType.Body) != 0))
                {
                    throw new Exception("Parameter of type Body cannot be used in GET or HEAD methods");
                }
            }
            return body.ToString();
        }
    }

    public enum Method
    {
        GET = 1,
        POST = 2,
        PUT = 4,
        DELETE = 8,
        HEAD = 16,
        SAFE = GET | HEAD
    }

    public enum ParamType
    {
        Query = 1,
        Body = 2,
        Header = 4,
        Segment = 8,
        Form = Query | Body,
        Uri = Segment | Query
    }

    public enum DataFormat
    {
        Xml,
        Json
    }

    internal class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;
        public StringWriterWithEncoding(StringBuilder sb, Encoding newEncoding) : base(sb) { encoding = newEncoding; }
        public override Encoding Encoding { get { return encoding ?? base.Encoding; } }
    }
}
