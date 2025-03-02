﻿using System.Text.Json.Serialization;
using System.Text.Json;


namespace HackerAPI.Model.Helpers
{
    public class UnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long unixTime))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTime);
            }
            throw new JsonException("Invalid Unix timestamp format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }
    }
}
