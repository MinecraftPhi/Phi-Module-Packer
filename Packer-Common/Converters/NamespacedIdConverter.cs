using Newtonsoft.Json;
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
                throw CreateException($"Expected string value, but found {reader.TokenType}.", reader);
            }
            else if(reader.Value is string id && !string.IsNullOrWhiteSpace(id))
            {
                return IdRegex.IsMatch(id) ? id : throw CreateException($"Namespaced ID must match {IdRegex}", reader);
            }
            else
            {
                throw CreateException("Non-empty namespaced ID required.", reader);
            }
        }

        private static readonly Regex IdRegex = new Regex(@"\A[a-z0-9._-]+:[a-z0-9._-]+\z");

        private static Exception CreateException(string message, JsonReader reader)
        {
            var info = (IJsonLineInfo)reader;
            return new JsonSerializationException($"'{message}', Path '{reader.Path}', line {info.LineNumber}, position {info.LinePosition}.", reader.Path, info.LineNumber, info.LinePosition, null);
        }

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
