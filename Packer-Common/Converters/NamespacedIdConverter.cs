using Newtonsoft.Json;
using Phi.Packer.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Phi.Packer.Common.Converters
{
    public class NamespacedIdConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw reader.CreateSerializationException($"Expected string value, but found {reader.TokenType}.");
            }
            else if(reader.Value is string id && !string.IsNullOrWhiteSpace(id))
            {
                return IdRegex.IsMatch(id) ? id : throw reader.CreateSerializationException($"Namespaced ID must match {IdRegex}");
            }
            else
            {
                throw reader.CreateSerializationException("Non-empty namespaced ID required.");
            }
        }

        private static readonly Regex IdRegex = new Regex(@"\A[a-z0-9._-]+:[a-z0-9._-]+\z");

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new JsonSerializationException("Namespaced ID must not be empty");
            }
            else if(IdRegex.IsMatch(value))
            {
                writer.WriteValue(value);
            }
            else
            {
                throw new JsonSerializationException($"Namespaced ID must match {IdRegex}");
            }
        }
    }
}
