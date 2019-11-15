using Newtonsoft.Json;
using NuGet.Protocol;
using NuGet.Versioning;
using Phi.Packer.Common.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Phi.Packer.Common
{
    public class Module
    {
        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(NamespacedIdConverter))]
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public SemanticVersion? Version { get; set; }

        public static readonly string ModuleFileName = "module.json";
        public static Module? Load(string? path = null)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                path = Directory.GetCurrentDirectory();
            }
            else if(!Path.IsPathFullyQualified(path))
            {
                path = Path.GetFullPath(path);
            }

            // If path points either to a directory or a file that isn't a module file
            if (Path.GetFileName(path) is string fileName && fileName != ModuleFileName)
            {
                // If path points to a file that isn't a module file
                if(File.Exists(path))
                {
                    path = Path.GetDirectoryName(path)!; // When the path points to a file it is guaranteed to have a directory
                }

                // Find module file at same location
                path = Path.Combine(path, ModuleFileName);
            }

            return JsonConvert.DeserializeObject<Module>(File.ReadAllText(path), new SemanticVersionConverter());
        }
    }
}
