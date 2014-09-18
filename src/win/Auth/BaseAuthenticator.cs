using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveItA.Rest.Auth
{
    public abstract class BaseAuthenticator 
    {
        protected ICredentials _credentials;

        public BaseAuthenticator(ICredentials credentials)
        {
            _credentials = credentials;
        }

        public abstract void Authenticate(RestRequest request);
    }
}
