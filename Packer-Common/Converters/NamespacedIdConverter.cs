using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Phi.Packer.Common.Converters
{
    public class NamespacedIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw CreateException($"Expected string value, but found {reader.TokenType}.", reader);
            }
            else if(reader.Value is string id && !string.IsNullOrEmpty(id))
            {
                return IdRegex.IsMatch(id) ? id : throw CreateException($"Namespaced ID must match {IdRegex}", reader);
            }
            else
            {
                throw CreateException("Non-empty namespaced ID required.", reader);
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

        private static readonly Regex IdRegex = new Regex(@"\A[a-z0-9._-]+:[a-z0-9._-]+\z");

        private static Exception CreateException(string message, JsonReader reader)
        {
            var info = (IJsonLineInfo)reader;
            return new JsonSerializationException($"'{message}', Path '{reader.Path}', line {info.LineNumber}, position {info.LinePosition}.", reader.Path, info.LineNumber, info.LinePosition, null);
        }
    }
}
