using JsonSubTypes;
using Newtonsoft.Json;
using NuGet.Versioning;
using Phi.Packer.Common.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Phi.Packer.Common
{
    public enum DependencyType
    {
        Module, Datapack, Resourcepack, Mod, Invalid
    }

    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(ModuleDependencyDefinition), DependencyType.Module)]
    [JsonSubtypes.KnownSubType(typeof(DatapackDependencyDefinition), DependencyType.Datapack)]
    [JsonSubtypes.KnownSubType(typeof(ResourcepackDependencyDefinition), DependencyType.Resourcepack)]
    [JsonSubtypes.KnownSubType(typeof(ModDependencyDefinition), DependencyType.Mod)]
    public class DependencyDefinition
    {
        [DefaultValue(DependencyType.Invalid)]
        public virtual DependencyType Type => DependencyType.Invalid;

        public string? Description { get; set; }

        internal DependencyDefinition() { }
    }

    public sealed class ModuleDependencyDefinition : DependencyDefinition
    {
        public override DependencyType Type => DependencyType.Module;

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(NamespacedIdConverter))]
        public string? Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SemanticVersion? Version { get; set; }

        [DefaultValue(true)]
        public bool Bundle { get; set; } = true;

        public ModuleDependencyDefinition() { }
    }

    public enum SourceType
    {
        Git, File
    }

    public abstract class RawPackDependencyDefinition : DependencyDefinition
    {
        [JsonProperty(Required = Required.Always)]
        public SourceType Source { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string? Url { get; set; }

        [DefaultValue(true)]
        public bool Bundle { get; set; } = true;

        internal RawPackDependencyDefinition() { }
    }

    public sealed class DatapackDependencyDefinition : RawPackDependencyDefinition
    {
        public override DependencyType Type => DependencyType.Datapack;

        public DatapackDependencyDefinition() { }
    }

    public sealed class ResourcepackDependencyDefinition : RawPackDependencyDefinition
    {
        public override DependencyType Type => DependencyType.Resourcepack;

        public ResourcepackDependencyDefinition() { }
    }

    public sealed class ModDependencyDefinition : DependencyDefinition
    {
        public override DependencyType Type => DependencyType.Mod;

        [JsonProperty(Required = Required.Always)]
        public string? Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string? Url { get; set; }

        public ModDependencyDefinition() { }
    }

}
