# letsencrypt-update
LetsEncrypt bot to automatically create an LetsEncrypt certificate for web server (especially uriegel/home-server)

## Prerequisites

``` 
sudo dnf install openssl-devel
```

## Executing the tool for the first time

Copy ```cert.json``` to current directory with following content:

```
{
    "account": "<account@domain.de>",
    "domains": [
        "<domain 1>", "<domain 2>", "<domain 3>"
    ],
    "data": {
        "CountryName": "<my country>",
        "State": "<my state",
        "Locality": "<my locality>",
        "Organization": "<my organization>",
        "OrganizationUnit": "<my organization unit>",
        "CommonName": "<my common name, main domain>"
    }
}

```

```
letsencrypt-cert -create
``` 
This lets letsencrypt-cert create an account.

## Errors:
At the moment you have to call ```sudo ~/.dotnet/tools/uwebcert -create -prod``` from the directory ```/etc/letsencrypt-uweb```

Webserver has to be restarted in order to take renewed certificate

## cron job running every day:

```
sudo ~/.dotnet/tools/uwebcert -prod
``` 
