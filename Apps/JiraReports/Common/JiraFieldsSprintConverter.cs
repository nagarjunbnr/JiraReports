using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiraReports.Common
{
    public class JiraFieldsSprintConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public JiraFieldsSprintConverter()
        {

        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JProperty property = JProperty.Load(reader);

            if (property.Value.Type == JTokenType.Null)
                return null;

            if(property.Value.Type == JTokenType.Array)
            {
                string jsonValue = property.Value.Children().First().ToString();
                Regex regex = new Regex(@"name=(?<sprint>\w+( \w+)? Sprint \d+),", RegexOptions.Singleline);
                Match match = regex.Match(jsonValue);

                if(match.Success)
                    return match.Groups["sprint"].Value;
            }


            return null;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(string);
    }
}
