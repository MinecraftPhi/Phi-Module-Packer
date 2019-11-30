using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Packer.Preprocessor
{
    /// <summary>
    /// Abstraction to allow files both in memory and on disk to reduce disk thrashing
    /// </summary>
    /// <remarks>Note that actions that write to these files are not thread safe. It is recommended to copy the file before editing it</remarks>
    public interface IFile
    {
        /// <summary>
        /// Path of file as if it was on the disk
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Forceably load file into memory
        /// </summary>
        /// <returns>Loaded file</returns>
        public Task<MemoryFile> AsMemoryFile();

        /// <summary>
        /// Forceably save file to disk
        /// </summary>
        /// <returns>File on disk</returns>
        public Task<DiskFile> AsDiskFile();

        /// <summary>
        /// Replace all lines in file with the supplied lines
        /// </summary>
        /// <param name="lines">Lines to write</param>
        /// <returns></returns>
        public Task WriteLines(IAsyncEnumerable<string> lines);

        /// <see cref="WriteLines(IAsyncEnumerable{string})"/>
        public Task WriteLines(IEnumerable<string> lines);

        /// <summary>
        /// Append the supplied lines to the file
        /// </summary>
        /// <param name="lines">Lines to append</param>
        /// <returns></returns>
        public Task AppendLines(IAsyncEnumerable<string> lines);

        /// <see cref="AppendLines(IAsyncEnumerable{string})"/>
        public Task AppendLines(IEnumerable<string> lines);

        /// <summary>
        /// Copy this file to the specified file
        /// </summary>
        /// <param name="file">Destination file to copy to</param>
        /// <returns></returns>
        public Task CopyTo(IFile file);

        /// <summary>
        /// Fetch the lines from the file
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<string> Lines();
    }

    /// <summary>
    /// A file that is cached in memory
    /// </summary>
    /// <remarks>The cached file may not exist on disk, to provide access to this file to external programs <see cref="AsDiskFile"/> </remarks>
    public class MemoryFile : IFile
    {
        public MemoryFile(string path, List<string> lines)
        {
            Lines = lines;
            Path = path;
        }

        public MemoryFile(string path)
        {
            Path = path;
            Lines = new List<string>();
        }

        public string Path { get; }

        /// <summary>
        /// Direct access to the underlying lines in memory
        /// </summary>
        public List<string> Lines { get; }

        IAsyncEnumerable<string> IFile.Lines() => Lines.ToAsyncEnumerable();

        public Task WriteLines(IAsyncEnumerable<string> lines)
        {
            Lines.Clear();
            return AppendLines(lines);
        }

        public Task WriteLines(IEnumerable<string> lines)
        {
            Lines.Clear();
            return AppendLines(lines);
        }

        public async Task AppendLines(IAsyncEnumerable<string> lines)
        {
            await foreach (string line in lines)
            {
                Lines.Add(line);
            }
        }

        public Task AppendLines(IEnumerable<string> lines)
        {
            Lines.AddRange(lines);
            return Task.CompletedTask;
        }

        public Task CopyTo(IFile file) => file == this ? Task.CompletedTask : file.WriteLines(Lines);

        public Task<MemoryFile> AsMemoryFile() => Task.FromResult(this);

        public async Task<DiskFile> AsDiskFile()
        {
            var file = new DiskFile(Path);
            await file.WriteLines(Lines);
            return file;
        }
    }

    /// <summary>
    /// A file that exists on the native file system
    /// </summary>
    /// <remarks>The lines of the file are not cached for multiple accesses, if access to the lines is required multiple times it is recommended to <see cref="AsMemoryFile"/></remarks>
    public class DiskFile : IFile
    {
        public DiskFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public async Task<MemoryFile> AsMemoryFile() => new MemoryFile(Path, await Lines().ToListAsync());

        public Task<DiskFile> AsDiskFile() => Task.FromResult(this);

        public async IAsyncEnumerable<string> Lines()
        {
            using var file = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, useAsync: true);
            using StreamReader fileStream = new StreamReader(file);
            while (await fileStream.ReadLineAsync() is string line)
            {
                yield return line;
            }
        }

        public async Task WriteLines(IAsyncEnumerable<string> lines)
        {
            using var destFile = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            using var outStream = new StreamWriter(destFile);
            await foreach (string line in lines)
            {
                await outStream.WriteLineAsync(line);
            }
        }

        public async Task WriteLines(IEnumerable<string> lines)
        {
            using var destFile = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            using var outStream = new StreamWriter(destFile);
            foreach (string line in lines)
            {
                await outStream.WriteLineAsync(line);
            }
        }

        public async Task AppendLines(IAsyncEnumerable<string> lines)
        {
            using var destFile = new FileStream(Path, FileMode.Append, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            using var outStream = new StreamWriter(destFile);
            await foreach (string line in lines)
            {
                await outStream.WriteLineAsync(line);
            }
        }

        public async Task AppendLines(IEnumerable<string> lines)
        {
            using var destFile = new FileStream(Path, FileMode.Append, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            using var outStream = new StreamWriter(destFile);
            foreach (string line in lines)
            {
                await outStream.WriteLineAsync(line);
            }
        }

        public async Task CopyTo(IFile file)
        {
            if (file == this) return;

            // Optimise direct file to file copies
            if(file is DiskFile diskFile)
            {
                if (diskFile.Path == Path) return;

                using var sourceFile = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, useAsync: true);
                using var destFile = new FileStream(diskFile.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, useAsync: true);

                await sourceFile.CopyToAsync(destFile);
            }
            else
            {
                await file.WriteLines(Lines());
            }
        }
    }
}
