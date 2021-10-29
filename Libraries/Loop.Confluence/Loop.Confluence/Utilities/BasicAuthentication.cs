using Loop.Confluence.IoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    [InstancePerRequest(typeof(IWebAuthentication))]

    public class BasicAuthentication : IWebAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private string UserPassword => $"{Username}:{Password}".ToBase64String();

        public BasicAuthentication(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public BasicAuthentication()
        {
        }

        public IEnumerable<(string key, string value)> GenerateAuthHeaders()
        {
            yield return ("Authorization", $"Basic {UserPassword}");
        }
    }
}
