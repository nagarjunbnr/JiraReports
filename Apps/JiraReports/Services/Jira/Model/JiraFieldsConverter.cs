using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraFieldsConverter : JsonConverter
    {
        public static readonly Type JiraFieldsType = typeof(JiraFields);
        public static readonly Dictionary<string, PropertyInfo> JiraFieldProperties = GetProperties();

        private static Dictionary<string, PropertyInfo> GetProperties()
        {
            Dictionary<string, PropertyInfo> propertyDictionary = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] properties = JiraFieldsType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                JsonPropertyAttribute propertyAttr = property.GetCustomAttribute<JsonPropertyAttribute>(true);
                string propertyName = property.Name;

                if (propertyAttr != null)
                {
                    propertyName = propertyAttr.PropertyName;
                }

                propertyDictionary[propertyName] = property;
            }

            return propertyDictionary;
        }

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return JiraFieldsType == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType,
          object existingValue, JsonSerializer serializer)
        {
            JiraFields fields = new JiraFields();

            JObject jsonObject = JObject.Load(reader);
            foreach (JProperty property in jsonObject.Properties())
            {
                if (JiraFieldProperties.TryGetValue(property.Name, out PropertyInfo classProperty))
                {
                    string jsonValue = property.Value.ToString();
                    classProperty.SetValue(fields, ParseValue(jsonValue, property.CreateReader(), classProperty));
                }
                else if (property.Name.StartsWith("customfield_", StringComparison.OrdinalIgnoreCase))
                {
                    fields.CustomFields[property.Name] = property.Value.ToString();
                }
            }

            return fields;
        }

        private object ParseValue(string value, JsonReader jsonReader, PropertyInfo classProperty)
        {
            JsonConverterAttribute jsonConverter = classProperty.GetCustomAttribute<JsonConverterAttribute>();
            if(jsonConverter != null)
            {
                JsonConverter converter = Activator.CreateInstance(jsonConverter.ConverterType) as JsonConverter;
                return converter.ReadJson(jsonReader, classProperty.PropertyType, null, null);
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(classProperty.PropertyType);

            if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
                return JsonConvert.DeserializeObject(value, classProperty.PropertyType);

            return typeConverter.ConvertFromString(value);
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
