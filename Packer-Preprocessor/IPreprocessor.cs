using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Packer.Preprocessor
{
    /// <summary>
    /// Common interface for preprocessors
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Process the supplied files, potentially asyncronously, and report on the progress
        /// </summary>
        /// <param name="files">List of files to process</param>
        /// <param name="progressBar">Progress bar to report progress to</param>
        /// <returns>The processed files</returns>
        public IAsyncEnumerable<(IOutputDefinition, IFile)> Process(IEnumerable<IFile> files, IProgressBar progressBar);
    }
}
