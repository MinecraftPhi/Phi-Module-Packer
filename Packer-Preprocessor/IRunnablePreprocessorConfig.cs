using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Phi.Packer.Preprocessor
{
    public interface IRunnablePreprocessorConfig
    {
        public IReadOnlyDictionary<string, JToken> Params { get; }

        public IEnumerable<KeyValuePair<string, IOutputDefinition>> Outputs { get; }

        public IOutputDefinition GetOutput(string name);

        public bool TryGetOutput(string name, [MaybeNullWhen(false)] out IOutputDefinition? output);
    }
}
