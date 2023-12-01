using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

namespace TinyCA.Commands;

[Command(Name = "version", Description = "Show version information")]
public class Version
{
    const string version = "1.1.3";
    [SuppressMessage("Performance", "CA1822")]
    public int OnExecute(CommandLineApplication _, IConsole console)
    {
        console.WriteLine($"tinyca version {version}");
        return 0;
    }
}