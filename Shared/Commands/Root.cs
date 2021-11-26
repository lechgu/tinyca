using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

namespace TinyCA.Commands;

[Command(Name = "tinica ", Description = "Tiny Certificate Authority")]
[Subcommand(typeof(Init), typeof(Issue), typeof(Version))]
public class Root
{
    public const string Dir = ".tinyca";
    public const string KeyFile = "key.pem";
    public const string CertFile = "cert.pem";
    public const string CertLabel = "CERTIFICATE";
    public const string KeyLabel = "RSA PRIVATE KEY";

    [SuppressMessage("Performance", "CA1822")]
    public int OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify at a subcommand.");
        app.ShowHelp();
        return 1;
    }

}