using Loop.Confluence.Exceptions;
using Loop.Confluence.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Loop.Confluence.Services
{
    public abstract class ConfluenceService
    {
        protected IWebClient WebClient { get; private set; }
        protected IHttpUtility HTTPUtility { get; private set; }
        protected IJsonSerializer Serializer { get; private set; }

        public ConfluenceService(IWebClient webClient, IHttpUtility httpUtility, IJsonSerializer serializer)
        {
            this.WebClient = webClient;
            this.HTTPUtility = httpUtility;
            this.Serializer = serializer;
        }


        protected IWebClientResponse Get(IConfluenceConfig config, string path, params (string key, string value)[] qs)
        {
            IWebClientResponse response = this.WebClient.Get(config.Authentication, HTTPUtility.Combine(config.APIBaseUrl, path), qs);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new ConfluenceNotAuthorizedException();
            }

            return response;
        }

        protected string GetJSON(IConfluenceConfig config, string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            params (string key, string value)[] qs)
        {
            using (IWebClientResponse response = Get(config, path, qs))
            {
                return GetStringFromResponse(response, expectedStatusCode);
            }
        }

        protected string GetStringFromResponse(IWebClientResponse response, HttpStatusCode expectedStatusCode)
        {
            using (Stream responseStream = response.GetStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();

                    /*
                    if (response.StatusCode == expectedStatusCode)
                    {
                    }

                    throw new Exception("Unhandled Exception!");

                    */
                }
            }
        }

        protected IWebClientResponse Post(IConfluenceConfig config, string path, object jsonObject)
        {
            IWebClientResponse response = this.WebClient.Post(config.Authentication, HTTPUtility.Combine(config.APIBaseUrl, path), jsonObject);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new ConfluenceNotAuthorizedException();
            }

            return response;
        }

        protected string PostJSON(IConfluenceConfig config, string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            object jsonObject = null)
        {
            using (IWebClientResponse response = Post(config, path, jsonObject))
            {
                return GetStringFromResponse(response, expectedStatusCode);
            }
        }

        protected IWebClientResponse Put(IConfluenceConfig config, string path, object jsonObject)
        {
            IWebClientResponse response = this.WebClient.Put(config.Authentication, HTTPUtility.Combine(config.APIBaseUrl, path), jsonObject);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new ConfluenceNotAuthorizedException();
            }

            return response;
        }

        protected string PutJSON(IConfluenceConfig config, string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            object jsonObject = null)
        {
            using (IWebClientResponse response = Put(config, path, jsonObject))
            {
                return GetStringFromResponse(response, expectedStatusCode);
            }
        }

        protected ObjectType Deserialize<ObjectType>(string json)
        {
            return this.Serializer.Deserialize<ObjectType>(json);
        }
    }
}
