using Loop.Confluence.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Services.Model
{
    public class ConfluencePage
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public ConfluencePageVersion Version { get; set; }

        [JsonSerializeProtected]
        protected ConfluencePageBody Body { get; set; }

        [JsonIgnore]
        public string Content => Body?.Storage?.Value;
    }


    public class ConfluencePageBody
    { 
        public ConfluencePageView Storage { get; set; }
    }

    public class ConfluencePageView
    { 
        public string Value { get; set; }
    }


}
