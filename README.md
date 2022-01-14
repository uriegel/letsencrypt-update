# UwebServerCert
LetsEncrypt bot to automatically create an LetsEncrypt certificate for UwebServer

## Prerequisites

``` 
sudo dnf install openssl-devel
```

## Executing the tool for the first time

Copy ```cert.json``` to current directory

```
sudo ~/.dotnet/tools/uwebcert -create -prod
``` 
## Errors:
At the moment you have to call ```sudo ~/.dotnet/tools/uwebcert -create -prod``` from the directory ```/etc/letsencrypt-uweb```

Webserver has to be restarted in order to take renewed certificate

## cron job running every day:

```
sudo ~/.dotnet/tools/uwebcert -prod
``` 
