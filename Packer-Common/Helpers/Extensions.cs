using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Common.Helpers
{
    public static class Extensions
    {
        public static T? As<T>(this object? obj)
            where T : class => obj as T;

        public static Exception CreateSerializationException(this JsonReader reader, string message)
        {
            var info = (IJsonLineInfo)reader;
            return new JsonSerializationException($"'{message}', Path '{reader.Path}', line {info.LineNumber}, position {info.LinePosition}.", reader.Path, info.LineNumber, info.LinePosition, null);
        }
    }
}
