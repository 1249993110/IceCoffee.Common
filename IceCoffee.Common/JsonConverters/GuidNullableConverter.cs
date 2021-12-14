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
    class GuidNullableConverter : JsonConverter<Guid?>
    {
        public override Guid? ReadJson(JsonReader reader, Type objectType, Guid? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return Guid.Parse(reader.Value?.ToString());
        }

        public override void WriteJson(JsonWriter writer, Guid? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
#else
    public class GuidNullableConverter : JsonConverter<Guid?>
    {
        public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var str = reader.GetString();
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return Guid.Parse(str);
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
#endif
}
