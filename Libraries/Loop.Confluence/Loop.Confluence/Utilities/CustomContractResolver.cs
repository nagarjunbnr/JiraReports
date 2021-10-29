using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public class CustomContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> defaultProperties = base.CreateProperties(type, memberSerialization);


            List<JsonProperty> properties = type
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<JsonSerializeProtectedAttribute>() != null)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .ToList();

            foreach(JsonProperty property in properties)
            {
                property.Writable = true;
                property.Readable = true;
                defaultProperties.Add(property);
            }

            return defaultProperties;
        }
    }


}
