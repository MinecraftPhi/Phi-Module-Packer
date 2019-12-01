using McMaster.NETCore.Plugins;
using NuGet.Versioning;
using Phi.Packer.Helper;
using Phi.Packer.Preprocessor;
using ShellProgressBar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Packer.Common
{
    public class PreprocessorLoader
    {
        public static readonly IReadOnlyList<Type> DefaultSharedTypes = new[]
        {
            typeof(IDependencyDefinition), typeof(IFile), typeof(ILineWriter), typeof(IOutputDefinition), typeof(IPreprocessor), typeof(IPreprocessorFactory), typeof(IRunnablePreprocessorConfig) 
        }.ToList().AsReadOnly();

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<SemanticVersion, IPreprocessorFactory>> PreprocessorFactoryCache = new ConcurrentDictionary<string, ConcurrentDictionary<SemanticVersion, IPreprocessorFactory>>();

        protected IEnumerable<IPreprocessorFactory> LoadFrom(string path, Type[] sharedTypes, IProgressBar progressBar)
        {
            IEnumerable<string> dlls;
            if(
                   Path.GetExtension(path) is string extension
                && extension == ".dll"
                && File.Exists(path)
            )
            {
                progressBar.WriteMessage($"Attempting to load from {path}");
                dlls = new[] { path };
            }
            else if(Directory.Exists(path))
            {
                progressBar.WriteMessage($"Searching directory {path} for assemblies");
                dlls = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            }
            else
            {
                progressBar.WriteLine($"File or folder does not exist: {path}");
                progressBar.Finish();
                return Enumerable.Empty<IPreprocessorFactory>();
            }

            var assemblies = dlls.Select(file =>
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(file);
                    progressBar.WriteLine($"Found assembly: {name}");
                    return (file, name);
                }
                catch
                {
                    progressBar.WriteLine($"Skipping non-assembly dll: {file}");
                    return (null!, null!);
                }
            }).Where(assembly => assembly.name != null).ToList();

            if (assemblies.Any())
            {
                progressBar.MaxTicks = assemblies.Count;
                progressBar.WriteMessage("Loading preprocessors from assemblies");
                return assemblies.AsParallel().SelectMany(assembly =>
                {
                    try
                    {
                        // Do not use WriteMessage here as multiple are running at once
                        progressBar.WriteLine($"Loading assembly: {assembly.name}");
                        var loader = PluginLoader.CreateFromAssemblyFile(assembly.file, sharedTypes: sharedTypes);
                        return loader.LoadDefaultAssembly().GetTypes().Where(t => typeof(IPreprocessorFactory).IsAssignableFrom(t));
                    }
                    finally
                    {
                        progressBar.Tick();
                    }
                }).Select(Activator.CreateInstance).Cast<IPreprocessorFactory>();
            }
            else
            {
                progressBar.WriteLine($"No assemblies found in {path}");
                progressBar.Finish();
                return Enumerable.Empty<IPreprocessorFactory>();
            }
        }

        public async Task<PreprocessorDependencies?> Load(string modulePath, PreprocessorConfig preprocessorConfig, IProgressBar progressBar)
        {
            try
            {
                progressBar.MaxTicks = 2;
                if (
                       preprocessorConfig is RunnablePreprocessorConfig runnable
                    && !string.IsNullOrWhiteSpace(runnable.Id)
                    && runnable.Version != null
                )
                {
                    progressBar.WriteMessage($"Searching for preprocessor: {runnable.Id} v{runnable.Version}");
                    IPreprocessorFactory? factory = null;
                    if (
                           !PreprocessorFactoryCache.TryGetValue(runnable.Id, out var versionCache)
                        || !versionCache.TryGetValue(runnable.Version, out factory)
                    )
                    {
                        if (runnable is LocalPreprocessorConfig local)
                        {
                            var path = Path.Combine(modulePath, "preprocessors", local.Folder ?? local.Id ?? "");
                            progressBar.WriteMessage($"Preprocessor not loaded, attempting to load from disk: {path}");
                            var factories = (await Task.Run(() => 
                                LoadFrom(
                                    path,
                                    DefaultSharedTypes.ToArray(),
                                    progressBar.Spawn(1, "Loading preprocessors", new ProgressBarOptions() { CollapseWhenFinished = true })
                                )
                            )).ToList();
                            progressBar.Tick();
                            progressBar.WriteMessage($"Loaded {factories.Count} preprocessor(s) from {path}");
                            foreach (IPreprocessorFactory f in factories)
                            {
                                if(f.Id is string id && f.Version is SemanticVersion version)
                                {
                                    var versions = PreprocessorFactoryCache.GetOrAdd(id, _ => new ConcurrentDictionary<SemanticVersion, IPreprocessorFactory>());
                                    if(id == runnable.Id && version == runnable.Version)
                                    {
                                        factory = versions.GetOrAdd(version, f);
                                    }
                                    else
                                    {
                                        versions.TryAdd(version, f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            progressBar.Tick();
                            progressBar.WriteMessage($"Unable to load preprocessor {runnable.Id} of type {runnable.Type}");
                            return null;
                        }
                    }
                    else
                    {
                        progressBar.Tick();
                        progressBar.WriteMessage($"Found cached preprocessor: {runnable.Id} v{runnable.Version}");
                    }

                    if(factory == null)
                    {
                        progressBar.WriteMessage($"Unable to load preprocessor: {runnable.Id} v{runnable.Version}");
                    }
                    else
                    {
                        progressBar.WriteMessage($"Successfully loaded preprocessor: {runnable.Id} v{runnable.Version}");
                        return factory.Create(runnable, new ProgressBarLineWriter(progressBar));
                    }
                }

                return null;
            }
            finally
            {
                progressBar.Tick();
            }
        }
    }
}
