using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AdOut.Extensions.Communication
{
    public class TypeInfoConverter : JsonConverter
    {
        private readonly Type[] _types;
        public TypeInfoConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var converters = serializer.Converters.Where(x => !(x is TypeInfoConverter)).ToArray();

            var jObject = JObject.FromObject(value);
            var typeProperty = new JProperty("ObjectType", value.GetType().Name);
            jObject.AddFirst(typeProperty);
            jObject.WriteTo(writer, converters);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }
}
