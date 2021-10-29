using Loop.Confluence.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Loop.Confluence.Utilities
{
    [SingleInstance(typeof(IWebClient))]
    public class WebClient : IWebClient
    {
        protected IJsonSerializer Serializer { get; private set; }

        public WebClient(IJsonSerializer serializer)
        {
            this.Serializer = serializer;
        }

        public IWebClientResponse Get(IWebAuthentication auth, string url, params (string key, string value)[] arguments)
        {
            if (arguments != null && arguments.Length > 0)
            {
                url = AddQueryString(url, arguments);
            }

            HttpWebRequest request = HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "PostmanRuntime/7.13.0";
            AddAuthHeaders(auth, request);

            try
            {
                return new WebClientResponse(request.GetResponse() as HttpWebResponse);
            }
            catch (WebException ex)
            {
                return new WebClientResponse(ex.Response as HttpWebResponse);
            }
        }

        public IWebClientResponse Post(IWebAuthentication auth, string url, object json)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.UserAgent = "PostmanRuntime/7.13.0";
            request.ContentType = "application/json";
            using (Stream requestStream = request.GetRequestStream())
            {
                using (StreamWriter writer = new StreamWriter(requestStream))
                {
                    writer.Write(this.Serializer.Serialize(json));
                }
            }

            AddAuthHeaders(auth, request);

            try
            {
                return new WebClientResponse(request.GetResponse() as HttpWebResponse);
            }
            catch (WebException ex)
            {
                return new WebClientResponse(ex.Response as HttpWebResponse);
            }
        }

        public IWebClientResponse Put(IWebAuthentication auth, string url, object json)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            request.Method = "PUT";
            request.UserAgent = "PostmanRuntime/7.13.0";
            request.ContentType = "application/json";
            using (Stream requestStream = request.GetRequestStream())
            {
                using (StreamWriter writer = new StreamWriter(requestStream))
                {
                    writer.Write(this.Serializer.Serialize(json));
                }
            }

            AddAuthHeaders(auth, request);

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
            return url + "?" + String.Join("&", arguments.Select(kv => $"{kv.key}={HttpUtility.UrlEncode(kv.value)}")); ;
        }

        private void AddAuthHeaders(IWebAuthentication auth, HttpWebRequest request)
        {
            foreach ((string key, string value) in auth.GenerateAuthHeaders())
            {
                request.Headers[key] = value;
            }
        }
    }
}
