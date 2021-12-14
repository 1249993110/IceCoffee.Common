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
    class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value?.ToString());
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToStringWithoutT());
        }
    }
#else
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if(str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return DateTime.Parse(str);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToStringWithoutT());
        }
    }
#endif
}
