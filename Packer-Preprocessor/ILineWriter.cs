using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Preprocessor
{
    /// <summary>
    /// Abstraction around writers that can write entire lines of text
    /// </summary>
    /// <remarks>The normal TextWriter is not used as that assumes single character writing</remarks>
    public interface ILineWriter
    {
        public void WriteLine(string message);
    }

    /// <summary>
    /// Supplies access to the line writing capabilities of a progress bar without exposing the other properties
    /// </summary>
    public class ProgressBarLineWriter : ILineWriter
    {
        private readonly IProgressBar progressBar;
        public ProgressBarLineWriter(IProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }

        public void WriteLine(string message) => progressBar.WriteLine(message);
    }
}
