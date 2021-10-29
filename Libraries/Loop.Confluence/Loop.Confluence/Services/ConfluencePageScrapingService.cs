using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Loop.Confluence.IoC;
using Loop.Confluence.Services.Model;
using Loop.Confluence.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Loop.Confluence.Services
{
    public interface IConfluencePageScrapingService
    {
        //ConfluencePageMetadata GetPageMetadata(IConfluenceConfig config, string url);
    }

    [SingleInstance(typeof(IConfluencePageScrapingService))]
    public class ConfluencePageScrapingService : ConfluenceService, IConfluencePageScrapingService
    {
        public ConfluencePageScrapingService(IWebClient webClient, IHttpUtility httpUtility, IJsonSerializer serializer)
            : base(webClient, httpUtility, serializer)
        {
            
        }

        //public ConfluencePageMetadata GetPageMetadata(IConfluenceConfig config, string url)
        //{
        //    string content;
        //    using (IWebClientResponse response = this.WebClient.Get(config.Authentication, url))
        //    {
        //        content = GetStringFromResponse(response, HttpStatusCode.OK);
        //    }

        //    Console.WriteLine(content);

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml2(content);

        //    Dictionary<string, HtmlNode> metaTags = doc.DocumentNode.QuerySelectorAll("meta")
        //        .Where(n => n.Attributes["name"] != null)
        //        .Where(n => n.Attributes["name"].Value.StartsWith("ajs-"))
        //        .ToDictionary(n => n.Attributes["name"].Value);

        //    Console.WriteLine(metaTags.Keys);

        //    return new ConfluencePageMetadata()
        //    {
        //        PageId = metaTags.TryGetValue("ajs-page-id", out HtmlNode metaNode) ? metaNode.Attributes["content"].Value : null,
        //        PageTitle = metaTags.TryGetValue("ajs-page-title", out metaNode) ? metaNode.Attributes["content"].Value : null,
        //        ParentPageId = metaTags.TryGetValue("ajs-parent-page-id", out metaNode) ? metaNode.Attributes["content"].Value : null,
        //        SpaceId = metaTags.TryGetValue("ajs-space-key", out metaNode) ? metaNode.Attributes["content"].Value : null,
        //    };
        //}

        //private IWebClientResponse GetCurrentUser(IConfluenceConfig config)
        //{
        //    return Get(config, "/rest/api/user/current");
        //}




    }
}
