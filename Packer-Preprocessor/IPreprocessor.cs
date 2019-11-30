using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Packer.Preprocessor
{
    /// <summary>
    /// Common interface for preprocessors
    /// </summary>
    /// <remarks>
    /// Any types that implement this interface must have a parameterless constructor in order to be constructable by the packer.
    /// </remarks>
    public interface IPreprocessor
    {
        /// <summary>
        /// Namespaced ID of this preprocessor
        /// </summary>
        public string Id { get; }

        public SemanticVersion Version { get; }

        /// <summary>
        /// Create a new instance of this preprocessor configured with the specified config
        /// </summary>
        /// <param name="config">Preprocessor config</param>
        /// <param name="log">Writer for logging parse errors</param>
        /// <returns><c>null</c> if the provided config is invalid for this preprocessor. Otherwise returns a new instance</returns>
        public IPreprocessor? Create(IRunnablePreprocessorConfig config, ILineWriter log);

        /// <summary>
        /// Process the supplied files, potentially asyncronously, and report on the progress
        /// </summary>
        /// <param name="files">List of files to process</param>
        /// <param name="progressBar">Progress bar to report progress to</param>
        /// <returns>The processed files</returns>
        public IAsyncEnumerable<(IOutputDefinition, IFile)> Process(IEnumerable<IFile> files, IProgressBar progressBar);
    }
}
