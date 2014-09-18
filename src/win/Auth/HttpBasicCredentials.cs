using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveItA.Rest.Auth
{
    public class HttpBasicCredentials : ICredentials
    {
        public HttpBasicCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
