using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Preprocessor
{
    public enum DependencyType
    {
        Module, Datapack, Resourcepack, Mod, Invalid
    }

    public interface IDependencyDefinition
    {
        public DependencyType Type { get; }
    }

    public interface IBundleableDependencyDefinition : IDependencyDefinition
    {
        public bool Bundle { get; }

        public string? Path { get; }
    }
}
