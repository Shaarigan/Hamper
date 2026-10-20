// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license

using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Soe.Runtime;

namespace Soe.Hamper
{
    /// <summary>
    /// Declares the host application for command line tools and services
    /// </summary>
    static partial class Hamper
    {
        [InlineArray(3)]
        private struct AvailableOptions
        {
            private const int DefaultCapacity = 3;
            private Option element0;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Span<Option> AsSpan()
            {
                return MemoryMarshal.CreateSpan(ref element0, DefaultCapacity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static AvailableOptions GetOptions()
        {
            AvailableOptions result = default;
            Span<Option> options = result.AsSpan();
            {
                options[0] = new Option("help", OptionFlags.Flag | OptionFlags.Optional, "Shows this help message");
                options[1] = new Option("h", OptionFlags.Alias, parameter: "help");
                options[2] = new Option("module", OptionFlags.Verb | OptionFlags.NoHeader, "A module loaded by this host. The module must contain an entry point");
            }
            return result;
        }
        
        /// <summary>
        /// Entry-point of the runtime host
        /// </summary>
        /// <param name="args">Parsed command line arguments</param>
        /// <returns>The exit code of the current run</returns>
        public static int Main(string[] args)
        {
            ParamSource parameter = new ParamSource(args);
            using (CommandReader<ParamSource> reader = new CommandReader<ParamSource>(parameter, CommandReaderOptions.AllowVerbValues | CommandReaderOptions.IgnoreCase | CommandReaderOptions.IgnoreMatch))
            {
                if (reader.Read(GetOptions().AsSpan()))
                {
                    switch (reader.Fragment.Type)
                    {
                        case CommandToken.Verb: return RunModule(reader.Fragment.RawValue.ToLowerInvariant(), ref parameter);
                        case CommandToken.Command: return ProcessFlags(reader);
                    }
                }
            }

            ShowManPage();
            return 0;
        }
        
        static int RunModule(string moduleName, ref ParamSource parameter)
        {
            throw new NotImplementedException();
        }

        static int ProcessFlags(CommandReader<ParamSource> reader)
        {
            ShowManPage();
            return 0;
        }

        static void ShowManPage()
        {
            Span<Option> options = GetOptions().AsSpan();
            Console.Write("hamper [module] ");
            Console.Write(InfoBuilder.Generate(options, headerPostfix: "[<args>]", terminalWidth: Console.WindowWidth));
        }
    }
}
