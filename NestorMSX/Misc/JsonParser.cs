using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Konamiman.NestorMSX.Misc
{
    public static class JsonParser
    {
        /// <summary>
        /// Parses a JSON string into an object. Arrays are converted to object[],
        /// objects are converted to Dictionary&lt;string, object&gt;.
        /// </summary>
        public static object Parse(string json)
        {
            var lines = json.Split('\r', '\n');
            lines = lines.Where(line => !line.Trim().StartsWith("//")).ToArray();
            json = string.Join("\r\n", lines);

            var token = JToken.Parse(json);
            return GetObjectFor(token);
        }

        private static object GetObjectFor(JToken token)
        {
            if (token is JObject)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var item in token as JObject)
                {
                    dictionary[item.Key] = GetObjectFor(item.Value);
                }
                return dictionary;
            }
            else if (token is JArray)
            {
                var list = new List<object>();
                foreach (var child in token.Children())
                {
                    list.Add(GetObjectFor(child));
                }
                return list.ToArray();
            }
            else
            {
                return (token as JValue).Value;
            }
        }
    }
}
