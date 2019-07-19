using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Settings
{
    class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value;
            JObject jo = new JObject();
            jo.Add("Address", JToken.FromObject(ep.Address, serializer));
            jo.Add("Port", ep.Port);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            return new IPEndPoint(address, port);
        }
    }
}
