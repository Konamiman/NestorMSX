﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Konamiman.NestorMSX.Misc
{
    public static class JsonParser
    {
        public static object Parse(string json)
        {
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
