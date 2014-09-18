using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveItA.Rest.Auth
{
    internal class OAuthToken
    {
        private int _expires_in;
        private DateTime _UTCExpiresOn;
        public int expires_in
        {
            get
        {
            return _expires_in;
        }
            set
            {
                _expires_in = value;
                _UTCExpiresOn = DateTime.UtcNow.AddSeconds(_expires_in);
            }
        }
        public DateTime UTCExpiresOn { get { return _UTCExpiresOn; } }

        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }
}
