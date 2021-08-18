using IceCoffee.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
#if NET45
using Newtonsoft.Json;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

namespace IceCoffee.Common.JsonConverters
{
#if NET45
    class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Guid.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
#else
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Guid.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
#endif
}
