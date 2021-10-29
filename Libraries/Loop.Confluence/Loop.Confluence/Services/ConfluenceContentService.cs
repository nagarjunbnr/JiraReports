using Loop.Confluence.IoC;
using Loop.Confluence.Services.Model;
using Loop.Confluence.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Loop.Confluence.Services
{
    public interface IConfluenceContentService
    {
        ConfluencePage GetPage(IConfluenceConfig config, string pageId);
        string SavePage(IConfluenceConfig config, string pageId, string title, string content, int version);
    }

    [SingleInstance(typeof(IConfluenceContentService))]
    public class ConfluenceContentService : ConfluenceService, IConfluenceContentService
    {
        private const string RelativeUrl = "/rest/api/content";

        public ConfluenceContentService(IWebClient webClient, IHttpUtility httpUtility, IJsonSerializer serializer)
            : base(webClient, httpUtility, serializer)
        {

        }


        public ConfluencePage GetPage(IConfluenceConfig config, string pageId)
        {
            List<(string, string)> parameters = new List<(string, string)>()
            {
                ( "expand", "body.storage,version" )
            };

            return Deserialize<ConfluencePage>(GetJSON(config, $"{RelativeUrl}/{pageId}", HttpStatusCode.OK, parameters.ToArray()));
        }

        public string SavePage(IConfluenceConfig config, string pageId, string title, string content, int version)
        {
            return PutJSON(config, $"{RelativeUrl}/{pageId}", HttpStatusCode.OK, new
            {
                version = new { number = version },
                title,
                type = "page",
                body = new
                {
                    storage = new
                    {
                        value = content,
                        representation = "storage"
                    }
                }
            });
        }
    }
}
