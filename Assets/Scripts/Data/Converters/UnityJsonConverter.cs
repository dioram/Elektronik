using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Elektronik.Data.Converters
{
    public class UnityJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var o = new JObject();
            switch (value)
            {
            case Vector3 v:
                o.Add("x", JToken.FromObject(v.x));
                o.Add("y", JToken.FromObject(v.y));
                o.Add("z", JToken.FromObject(v.z));
                break;
            case Quaternion q:
                o.Add("x", JToken.FromObject(q.x));
                o.Add("y", JToken.FromObject(q.y));
                o.Add("z", JToken.FromObject(q.z));
                o.Add("w", JToken.FromObject(q.w));
                break;
            case Color c:
                o.Add("r", JToken.FromObject(c.r));
                o.Add("g", JToken.FromObject(c.g));
                o.Add("b", JToken.FromObject(c.b));
                o.Add("a", JToken.FromObject(c.a));
                break;
            }
            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JsonUtility.FromJson((string) reader.Value, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3) || objectType == typeof(Quaternion) || objectType == typeof(Color);
        }
    }
}