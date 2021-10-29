using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Services.Model
{
    public class ConfluenceServiceResult<TResult>
    {
        public List<TResult> Results { get; set; }
        public int Start { get; set; }
        public int Limit { get; set; }
        public int Size { get; set; }
    }
}
