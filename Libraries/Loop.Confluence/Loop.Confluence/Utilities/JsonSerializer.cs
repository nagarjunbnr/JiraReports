using Loop.Confluence.IoC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    [SingleInstance(typeof(IJsonSerializer))]
    public class JsonSerializer : IJsonSerializer
    {
        public TObject Deserialize<TObject>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomContractResolver();
            return JsonConvert.DeserializeObject<TObject>(json, settings);
        }

        public string Serialize(object objectToSerialize)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(objectToSerialize, settings);
        }
    }
}
