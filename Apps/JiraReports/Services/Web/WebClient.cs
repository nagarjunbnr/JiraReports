using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JiraReports.Services.Web
{
    [InstancePerRequest(typeof(IWebClient))]
    public class WebClient : IWebClient
    {
        public IWebAuthentication Authentication {get; set; }

        public WebClient()
        {
        }

        public WebClient(IWebAuthentication auth)
        {
            this.Authentication = auth;
        }

        public IWebClientResponse Get(string url, params (string key, string value)[] arguments)
        {
            if(arguments != null && arguments.Length > 0)
            {
                url = AddQueryString(url, arguments);
            }

            HttpWebRequest request = HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            request.Method = "GET";
            AddAuthHeaders(request);

            try
            {
                return new WebClientResponse(request.GetResponse() as HttpWebResponse);
            }
            catch(WebException ex)
            {
               return new WebClientResponse(ex.Response as HttpWebResponse);
            }
        }

        public IWebClientResponse Post(string url, object json)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            using(Stream requestStream = request.GetRequestStream())
            {
                using(StreamWriter writer = new StreamWriter(requestStream))
                {
                    writer.Write(JsonConvert.SerializeObject(json));
                }
            }

            AddAuthHeaders(request);

            try
            {
                return new WebClientResponse(request.GetResponse() as HttpWebResponse);
            }
            catch (WebException ex)
            {
                return new WebClientResponse(ex.Response as HttpWebResponse);
            }
        }

        private string AddQueryString(string url, (string key, string value)[] arguments)
        {
            return url + "?" + String.Join("&", arguments.Select(kv => $"{kv.key}={HttpUtility.UrlEncode(kv.value)}"));;
        }

        private void AddAuthHeaders(HttpWebRequest request)
        {
            foreach ((string key, string value) in Authentication.GenerateAuthHeaders())
            {
                request.Headers[key] = value;
            }
        }
    }
}
