using JiraReports.Common;
using JiraReports.Services.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public abstract class JiraService
    {
        private const string JiraBaseURL = @"https://jira.affinitiv.com/rest/api/2/";

        protected IWebClient WebClient { get; private set; }
        protected IHttpUtility HTTPUtility { get; private set; }

        public JiraService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
        {
            this.WebClient = webClient;
            this.WebClient.Authentication = auth;
            this.HTTPUtility = httpUtility;
        }


        protected IWebClientResponse JiraGet(string path, params (string key, string value)[] qs)
        {
            IWebClientResponse response = this.WebClient.Get(HTTPUtility.Combine(JiraBaseURL, path), qs);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new JiraNotAuthorizedException();
            }

            return response;
        }

        protected string JiraGetJSON(string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            params (string key, string value)[] qs)
        {
            using (IWebClientResponse response = JiraGet(path, qs))
            {
                using (Stream responseStream = response.GetStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        if (response.StatusCode == expectedStatusCode)
                        {
                            return reader.ReadToEnd();
                        }

                        throw new Exception("Unhandled Exception!");
                    }
                }
            }
        }


        protected IWebClientResponse JiraPost(string path, object jsonObject)
        {
            IWebClientResponse response = this.WebClient.Post(HTTPUtility.Combine(JiraBaseURL, path), jsonObject);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new JiraNotAuthorizedException();
            }

            return response;
        }

        protected string JiraPostJSON(string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            object jsonObject = null)
        {
            using (IWebClientResponse response = JiraPost(path, jsonObject))
            {
                using (Stream responseStream = response.GetStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        if (response.StatusCode == expectedStatusCode)
                        {
                            return reader.ReadToEnd();
                        }

                        throw new Exception("Unhandled Exception!");
                    }
                }
            }
        }

        protected ObjectType Deserialize<ObjectType>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            return JsonConvert.DeserializeObject<ObjectType>(json, settings);
        }

        protected object DeserializeArray<ObjectType>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            return JsonConvert.DeserializeObject(json); //DeserializeObject<JsonArrayAttribute>(json, settings);
        }

    }
}
