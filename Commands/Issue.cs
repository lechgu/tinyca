using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using McMaster.Extensions.CommandLineUtils;

namespace TinyCA.Commands;

[Command(Name = "issue", Description = "Issue and sign a certificate")]
class Issue
{
    private const string serverAuthOid = "1.3.6.1.5.5.7.3.1";
    private const string clientAuthOid = "1.3.6.1.5.5.7.3.2";
    [Option(ShortName = "n", LongName = "name", Description = "a common name for the certificate (CN)")]
    [Required]
    public string? Name { get; set; }

    [Option(ShortName = "x", LongName = "expiry", Description = "certificate expiry, in days, default: 1 year")]
    public int? Expiry { get; set; }

    [Option(LongName = "dns", ShortName = "d", Description = "a dns name to be added to the certificate (SAN)")]
    public string[]? Dns { get; set; }

    [Option(LongName = "ip", ShortName = "i", Description = "an IP address to be added to the certificate (SAN)")]
    public String[]? Ip { get; set; }

    [SuppressMessage("Performance", "CA1822")]
    public int OnExecute(CommandLineApplication _, IConsole console)
    {
        if (!Directory.Exists(Root.Dir))
        {
            console.WriteLine($"Directory {Root.Dir} does not exist, run init first.");
            return 1;
        }

        if (Directory.Exists(Name))
        {
            console.WriteLine($"Directory {Name} already exists.");
            return 1;
        }
        if (Ip is not null)
        {
            foreach (var ip in Ip)
            {
                if (!IPAddress.TryParse(ip, out IPAddress _))
                {
                    console.WriteLine($"Invalid ip format {ip}. ");
                    return 1;
                }
            }
        }
        using var rsa = RSA.Create(2048);
        Directory.CreateDirectory(Name!);
        var keyFileName = Path.Join(Name, Root.KeyFile);
        File.WriteAllText(keyFileName, CreateKey(rsa));
        var certFileName = Path.Join(Name, Root.CertFile);
        File.WriteAllText(certFileName, CreateCertificate(rsa));




        return 0;
    }

    private string CreateCertificate(RSA rsa)
    {
        var certFileName = Path.Join(Root.Dir, Root.CertFile);
        var keyFileName = Path.Join(Root.Dir, Root.KeyFile);
        var issuer = X509Certificate2.CreateFromPemFile(certFileName, keyFileName);
        var dnNme = new X500DistinguishedName($"CN={Name}");
        var sanBuilder = new SubjectAlternativeNameBuilder();
        if (Dns is not null)
        {
            foreach (var dns in Dns)
            {
                sanBuilder.AddDnsName(dns);
            }
        }
        if (Ip is not null)
        {
            foreach (var ip in Ip)
            {
                var addr = IPAddress.Parse(ip);
                sanBuilder.AddIpAddress(addr);
            }
        }
        var request = new CertificateRequest(dnNme, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(sanBuilder.Build());
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(
                        new X509KeyUsageExtension(
                            X509KeyUsageFlags.DigitalSignature |
                            X509KeyUsageFlags.KeyEncipherment
                            , true));
        var enhancedKeyUsages = new OidCollection
            {
                new Oid(serverAuthOid),
                new Oid(clientAuthOid),
            };
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(enhancedKeyUsages, true)
        );
        var validFrom = DateTime.UtcNow;
        var validUntil = Expiry.HasValue ? validFrom.AddDays(Expiry.Value) : validFrom.AddYears(1);
        var certificate = request.Create(
            issuer,
            new DateTimeOffset(validFrom),
            new DateTimeOffset(validUntil),
            Guid.NewGuid().ToByteArray()
        );
        var cert = PemEncoding.Write(Root.CertLabel, certificate.Export(X509ContentType.Cert));
        return $"{new string(cert)}{Environment.NewLine}";
    }

    private static string CreateKey(RSA rsa)
    {
        var key = PemEncoding.Write(Root.KeyLabel, rsa.ExportRSAPrivateKey());
        return $"{new string(key)}{Environment.NewLine}";
    }
}