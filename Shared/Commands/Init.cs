using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using McMaster.Extensions.CommandLineUtils;

namespace TinyCA.Commands;

[Command(Name = "init", Description = "Initialize CA's certificate and signing key")]
public class Init
{
    [Option(ShortName = "x", LongName = "expiry", Description = "certificate expiry, in days, default: 10 years")]
    public int? Expiry { get; set; }

    [SuppressMessage("Performance", "CA1822")]
    public int OnExecute(CommandLineApplication _, IConsole console)
    {

        if (Directory.Exists(Root.Dir))
        {
            console.WriteLine($"Directory {Root.Dir} already exists.");
            return 1;
        }
        using var rsa = RSA.Create(2048);
        Directory.CreateDirectory(Root.Dir);
        var keyFileName = Path.Join(Root.Dir, Root.KeyFile);
        File.WriteAllText(keyFileName, CreateKey(rsa));
        var certFileName = Path.Join(Root.Dir, Root.CertFile);
        File.WriteAllText(certFileName, CreateCertificate(rsa));

        return 0;
    }

    private static string CreateKey(RSA rsa)
    {
        var key = PemEncoding.Write(Root.KeyLabel, rsa.ExportRSAPrivateKey());
        return $"{new string(key)}{Environment.NewLine}";
    }

    private string CreateCertificate(RSA rsa)
    {
        var dnNme = new X500DistinguishedName($"CN=tinyca root ca [{Guid.NewGuid()}]");
        var request = new CertificateRequest(dnNme, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(
                        new X509KeyUsageExtension(
                            X509KeyUsageFlags.DigitalSignature |
                            X509KeyUsageFlags.KeyCertSign
                            , true));
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true));
        var validFrom = DateTime.Now;
        var validUntil = Expiry.HasValue ? validFrom.AddDays(Expiry.Value) : validFrom.AddYears(10);
        var certificate = request.CreateSelfSigned(new DateTimeOffset(validFrom), new DateTimeOffset(validUntil));

        var cert = PemEncoding.Write(Root.CertLabel, certificate.Export(X509ContentType.Cert));
        return $"{new string(cert)}{Environment.NewLine}";
    }

}