using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Preprocessor
{
    public interface IOutputDefinition
    {
        public string? Folder { get; }

        public IEnumerable<IDependencyDefinition> Dependencies { get; }

        public IEnumerable<IDependencyDefinition> OptionalDependencies { get; }
    }
}
