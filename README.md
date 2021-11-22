# tinyca

Tinyca is a simple Certification Authority. using Tinyca is appropriate to generate certificates to secure non-customer facing network traffic, for instance for the gRPC micro-services.
Upon initialization, tinyca creates a self-signed certificate which is used subsequently for issuing and signing the child certificates.
Tinyca runs on Windows, Mac OS X and on Linux.

## Building tinyca

To build tinyca you need to have
[.net 6 sdk](https://dotnet.microsoft.com/download/dotnet/6.0)
installed for your system

```
dotnet build
```

Supplied `makefile` illustrates how to build standalone, self-contained optimized versions of tinyca, for example on Mac OS X

```
dotnet publish -c release -r osx-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true

cp bin/release/net6.0/osx-x64/publish/tinyca .
```

## Running tinyca

```
tinyca init
```

This will create a directory `.tinyca` which contains a self-signed certificate used by the Certificate Authority. By default, the certificate is valid for 10 years.

Once you have initialized the Certificate Authority, you can issue child certificates like this

```
tinyca issue --name localhost --dns localhost --ip 127.0.0.1
```

This creates a directory `localhost` that contains a certificate and corresponding key valid for the DNS name `localhost` and IP address `127.0.0.1`. Multiple DNS names and/or IP addresses can be provided, if desired. Such child certificate is valid, by default, for one year.
