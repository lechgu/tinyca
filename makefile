.SILENT: check
.PHONY: check
check:
	openssl x509 -noout -modulus -in .tinyca/cert.pem | openssl md5
	openssl rsa -noout -modulus -in .tinyca/key.pem | openssl md5
osx:
	dotnet publish -c release -r osx-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
	cp bin/release/net6.0/osx-x64/publish/tinyca .
linux:
	dotnet publish -c release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
	cp bin/release/net6.0/linux-x64/publish/tinyca .
win:
	dotnet publish -c release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
	cp bin/release/net6.0/win-x64/publish/tinyca.exe .
