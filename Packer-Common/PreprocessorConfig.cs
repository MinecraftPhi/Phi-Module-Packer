using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using JsonSubTypes;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using NuGet.Versioning;
using Phi.Packer.Common.Converters;
using Newtonsoft.Json.Linq;
using DotNet.Globbing;

namespace Phi.Packer.Common
{
    public enum PreprocessorType
    {
        Verified, Nuget, Local, Custom, Split, Invalid
    }

    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(VerifiedPreprocessor), PreprocessorType.Verified)]
    [JsonSubtypes.KnownSubType(typeof(NugetPreprocessor), PreprocessorType.Nuget)]
    [JsonSubtypes.KnownSubType(typeof(LocalPreprocessor), PreprocessorType.Local)]
    [JsonSubtypes.KnownSubType(typeof(CustomPreprocessor), PreprocessorType.Custom)]
    [JsonSubtypes.KnownSubType(typeof(SplitPreprocessor), PreprocessorType.Split)]
    public class PreprocessorConfig
    {
        [DefaultValue(PreprocessorType.Invalid)]
        public virtual PreprocessorType Type => PreprocessorType.Invalid;

        [JsonIgnore]
        public bool CanRun { get; }

        public List<string> Profiles { get; } = new List<string>();

        // Limit subclasses to this assembly
        internal PreprocessorConfig(bool canRun) {
            CanRun = canRun;
        }

        internal PreprocessorConfig()
        {
            CanRun = false;
        }
    }

    public class RunnablePreprocessor : PreprocessorConfig
    {

        public IDictionary<string, JToken> Params { get; } = new Dictionary<string, JToken>();

        public List<Glob> Inputs { get; } = new List<Glob>();

        public IDictionary<string, OutputDefinition> Outputs { get; } = new Dictionary<string, OutputDefinition>();

        internal RunnablePreprocessor() : base(true) { }
    }

    public sealed class VerifiedPreprocessor : RunnablePreprocessor
    {
        public override PreprocessorType Type => PreprocessorType.Verified;

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(NamespacedIdConverter))]
        public string? Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SemanticVersion? Version { get; set; }

        public VerifiedPreprocessor() { }
    }

    public sealed class NugetPreprocessor : RunnablePreprocessor
    {
        public override PreprocessorType Type => PreprocessorType.Nuget;

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(NamespacedIdConverter))]
        public string? Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SemanticVersion? Version { get; set; }

        public NugetPreprocessor() { }
    }

    public sealed class LocalPreprocessor : RunnablePreprocessor
    {
        public override PreprocessorType Type => PreprocessorType.Local;

        [JsonProperty(Required = Required.Always)]
        public string? Folder { get; set; }

        public LocalPreprocessor() { }
    }

    public sealed class CustomPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Custom;

        [JsonProperty(Required = Required.Always)]
        public string? Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string? Url { get; set; }

        public CustomPreprocessor() { }
    }

    public sealed class SplitPreprocessor : RunnablePreprocessor
    {
        public override PreprocessorType Type => PreprocessorType.Split;

        public IDictionary<string, PreprocessorConfig> Preprocessors { get; } = new Dictionary<string, PreprocessorConfig>();

        public SplitPreprocessor() { }
    }
}
