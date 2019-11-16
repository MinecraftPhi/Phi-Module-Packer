using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using JsonSubTypes;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

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

    public sealed class VerifiedPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Verified;

        public VerifiedPreprocessor() : base(true) { }
    }

    public sealed class NugetPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Nuget;

        public NugetPreprocessor() : base(true) { }
    }

    public sealed class LocalPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Local;

        public LocalPreprocessor() : base(true) { }
    }

    public sealed class CustomPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Custom;

        public string? Name { get; set; }

        public string? Url { get; set; }
    }

    public sealed class SplitPreprocessor : PreprocessorConfig
    {
        public override PreprocessorType Type => PreprocessorType.Split;

        public IDictionary<string, PreprocessorConfig> Preprocessors { get; } = new Dictionary<string, PreprocessorConfig>();

        public SplitPreprocessor() : base(true) { }
    }
}
