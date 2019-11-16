using DotNet.Globbing;
using Newtonsoft.Json;
using Phi.Packer.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Phi.Packer.Common.Converters
{
    public class GlobConverter : JsonConverter<Glob>
    {
        public override Glob ReadJson(JsonReader reader, Type objectType, Glob? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw reader.CreateSerializationException($"Expected string glob value, but found {reader.TokenType}.");
            }
            else if (reader.Value is string glob && !string.IsNullOrWhiteSpace(glob))
            {
                return Glob.Parse(glob);
            }
            else
            {
                throw reader.CreateSerializationException("Empty glob value");
            }
        }

        public override void WriteJson(JsonWriter writer, Glob? value, JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
