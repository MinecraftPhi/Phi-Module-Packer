using Newtonsoft.Json;
using Phi.Packer.Preprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phi.Packer.Common
{
    public sealed class OutputDefinition : IOutputDefinition
    {
        public string? Folder { get; set; }

        public List<PreprocessorConfig> Preprocessors { get; } = new List<PreprocessorConfig>();

        public List<string> Profiles { get; } = new List<string>();

        public List<DependencyDefinition> Dependencies { get; } = new List<DependencyDefinition>();

        public List<DependencyDefinition> OptionalDependencies { get; } = new List<DependencyDefinition>();

        IEnumerable<IDependencyDefinition> IOutputDefinition.Dependencies => Dependencies.Cast<IDependencyDefinition>();

        IEnumerable<IDependencyDefinition> IOutputDefinition.OptionalDependencies => OptionalDependencies.Cast<IDependencyDefinition>();
    }
}
