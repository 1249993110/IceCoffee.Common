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
    class StringConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString();
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if(reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
            {
                return reader.Value.ToString();
            }

            throw new JsonException();
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
#else
    public class StringConverter : System.Text.Json.Serialization.JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if(reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal().ToString();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
#endif
}