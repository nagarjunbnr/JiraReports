using Loop.Confluence.IoC;
using Loop.Confluence.Services.Model;
using Loop.Confluence.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Loop.Confluence.Services
{
    public interface IConfluenceSpaceService
    {
        IEnumerable<ConfluenceSpace> GetSpaces(IConfluenceConfig config);
        IEnumerable<ConfluencePage> GetPagesForSpace(IConfluenceConfig config, string spaceId);
    }

    [SingleInstance(typeof(IConfluenceSpaceService))]
    public class ConfluenceSpaceService : ConfluenceService, IConfluenceSpaceService
    {
        private const string RelativeUrl = "/rest/api/space";

        public ConfluenceSpaceService(IWebClient webClient, IHttpUtility httpUtility, IJsonSerializer serializer)
            : base(webClient, httpUtility, serializer)
        {

        }

        public IEnumerable<ConfluenceSpace> GetSpaces(IConfluenceConfig config)
        {
            ConfluenceSpaceResult result = null;
            int start = 0;
            do
            {
                List<(string, string)> parameters = new List<(string, string)>()
                {
                    ( "start", start.ToString() )
                };

                result = Deserialize<ConfluenceSpaceResult>(GetJSON(config, RelativeUrl, HttpStatusCode.OK, parameters.ToArray()));
                start += result.Size;

                foreach(ConfluenceSpace space in result.Results)
                {
                    yield return space;
                }

            } while (result.Limit == result.Size);
        }

        public IEnumerable<ConfluencePage> GetPagesForSpace(IConfluenceConfig config, string spaceId)
        {
            ConfluencePageResult result = null;
            int start = 0;
            do
            {
                List<(string, string)> parameters = new List<(string, string)>()
                {
                    ( "start", start.ToString() )
                };

                result = Deserialize<ConfluencePageResult>(GetJSON(config, $"{RelativeUrl}/{spaceId}/content/page", HttpStatusCode.OK, parameters.ToArray()));
                start += result.Size;

                foreach (ConfluencePage page in result.Results)
                {
                    yield return page;
                }

            } while (result.Limit == result.Size);
        }

    }
}
