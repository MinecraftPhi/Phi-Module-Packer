using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phi.Packer.Preprocessor
{
    /// <summary>
    /// Factory that can create and configure instances of a specific preprocessor ID and version
    /// </summary>
    /// <remarks>
    /// Any types that implement this interface must have a parameterless constructor in order to be constructable by the packer.
    /// </remarks>
    public interface IPreprocessorFactory
    {
        /// <summary>
        /// Namespaced ID of this preprocessor
        /// </summary>
        public string Id { get; }

        public SemanticVersion Version { get; }

        public string ConfigPath { get; }

        /// <summary>
        /// Create a new instance of this preprocessor configured with the specified config
        /// </summary>
        /// <remarks>
        /// Preprocessors may require extra dependencies based on the custom parameters and outputs.
        /// These extra dependencies are declared in the <c>ConfigPath</c> file and specifed in the return value by the ID given in the file.
        /// 
        /// The returned <c>IOutputDefinition</c>s are required to be retrieved from the supplied config, do not create new instances
        /// </remarks>
        /// <param name="config">Preprocessor config</param>
        /// <param name="log">Writer for logging parse errors</param>
        /// <returns><c>null</c> if the provided config is invalid for this preprocessor. Otherwise returns a value containing the new instance and the dependencies for the given config</returns>
        public PreprocessorDependencies? Create(IRunnablePreprocessorConfig config, ILineWriter log);
    }

    /// <summary>
    /// Connects a configured instance of a preprocessor with it's dependencies
    /// </summary>
    public struct PreprocessorDependencies
    {
        /// <summary>
        /// The configured instance of the preprocessor
        /// </summary>
        public IPreprocessor Instance { get; set; }

        /// <summary>
        /// The extra dependency IDs for each output
        /// </summary>
        public IGrouping<IOutputDefinition, (string id, bool required)> OutputDependencies { get; set; }
    }
}
