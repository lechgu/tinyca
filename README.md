# TinyCA

TinyCA is a simple Certification Authority. using TinyCA is appropriate to generate certificates to secure non-customer facing network traffic, for instance for the gRPC micro-services.
Upon initialization, tinyCA creates a self-signed certificate which is used subsequently for issuing and signing the child certificates.
TinyCA runs on Windows, Mac OS X and on Linux.

## Installing TinyCA

The simplest way to install TinyCA, if you have .net framework 6.0 or 7.0 installed on the machine is to install it as a tool

```
dotnet tool install --global TinyCA
```

It is also possible to build a self-contained standalone version of TinyCA for your platform.
For example, for MacOS X do

```
cd Standalone
dotnet publish -c Release -r osx-x64 -f net7.0 -o .
```

This will generate a binary `tinyca`. This binary will run even the .net framework is not installed on the destination machine.

For the correct values of the `-r` parameter, refer [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

Specify the .Net version with the `-f` parameter.

## Running TinyCA

```
tinyca init
```

This will create a directory `.tinyca` which contains a self-signed certificate used by the Certificate Authority. By default, the certificate is valid for 10 years.

Once you have initialized the Certificate Authority, you can issue child certificates like this

```
tinyca issue --name localhost --dns localhost --ip 127.0.0.1
```

This creates a directory `localhost` that contains a certificate and corresponding key valid for the DNS name `localhost` and IP address `127.0.0.1`. Multiple DNS names and/or IP addresses can be provided, if desired. Such child certificate is valid, by default, for one year.

## Making the system to trust certificates signed by TinyCA

#### Mac OSX

In Mac Terminal, run elevated (with `sudo`):

```
security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain .tinyca/cert.pem
```

#### Windows

In the Windows terminal, run elevated ("as administrator"):

```
certutil -addstore Root .tinyca\cert.pem

```
