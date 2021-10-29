using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IJsonSerializer
    {
        string Serialize(object objectToSerialize);
        TObject Deserialize<TObject>(string json);
    }
}
