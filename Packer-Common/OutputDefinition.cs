using Newtonsoft.Json;
using Phi.Packer.Common.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Common
{
    public sealed class OutputDefinition
    {
        public string? Folder { get; set; }

        public List<PreprocessorConfig> Preprocessors { get; } = new List<PreprocessorConfig>();

        public List<string> Profiles { get; } = new List<string>();
    }
}
