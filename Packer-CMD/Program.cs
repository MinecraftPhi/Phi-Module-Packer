using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NuGet.Protocol;
using Phi.Packer.Common;
using Phi.Packer.Common.Helpers;
using System;
using System.Linq;
using System.Reflection;
using Module = Phi.Packer.Common.Module;

namespace Phi.Packer.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "new":
                            CreateNew(args[1..]);
                            break;
                        case "build":
                            Build();
                            break;
                        case "watch":
                            Watch();
                            break;
                        default:
                            UnknownCommand(args[0]);
                            break;
                    }
                }
                else
                {
                    PrintHelp();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@$"Failed to run command:
{string.Join(" ", args)}

Error:
{e}");
            }
        }

        private static void PrintHelp(string? error = null)
        {
            Console.Error.Write(@$"
Phi Module Packer {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}
Copyright (c) 2019 MinecraftPhi
".TrimStart());
            if(!string.IsNullOrWhiteSpace(error))
            {
                Console.Error.Write(@$"
ERROR:
  {error}
");
            }
            Console.Error.Write(@"
  new module <name>          Scaffold a new module called <name>
  new preprocessor <name>    Scaffold a new C# solution+project for a pre-processor
  build                      Build the module in the current folder
  watch                      Continuously watch for changes to the module in the current folder and rebuild
");
        }

        private static void UnknownCommand(string cmd) => PrintHelp($"Unknown command '{cmd}'");

        private static void Watch()
        {
            throw new NotImplementedException();
        }

        private static void CreateNew(string[] args)
        {
            throw new NotImplementedException();
        }

        private static void Build()
        {
            try
            {
                if (Module.Load() is Module module)
                {
                    var json = JsonConvert.SerializeObject(module, Formatting.Indented, Module.JsonSettings);
                    Console.WriteLine(json);
                }
                else
                {
                    Console.Error.WriteLine($"Failed to build: Unknown parsing error");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($@"
Failed to build:
Invalid {Module.ModuleFileName}: {e.Message}
".TrimStart());
            }
        }
    }
}
