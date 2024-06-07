using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SaveBattle.Data;

namespace SaveBattle
{
    /// <summary>
    /// Supports serializing a <see cref="SaveId"/> as a key on integer value dictionaries.
    /// </summary>
    public class SaveIdDictionaryConverter : JsonConverter<Dictionary<SaveId, int>>
    {
        public override Dictionary<SaveId, int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = new Dictionary<SaveId, int>();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                reader.Read();
                reader.Read();
                var key = JsonSerializer.Deserialize<SaveId>(ref reader, options);

                reader.Read();
                var value = JsonSerializer.Deserialize<int>(ref reader, options);

                reader.Read();

                dictionary.Add(key, value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<SaveId, int> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var kvp in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("Key");
                JsonSerializer.Serialize(writer, kvp.Key, options);

                writer.WritePropertyName("Value");
                JsonSerializer.Serialize(writer, kvp.Value, options);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
