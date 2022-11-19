# UwebServerCert
LetsEncrypt bot to automatically create an LetsEncrypt certificate for UwebServer

'``

## Installation
```
dotnet tool install LetsencryptCert --global
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
LetsencryptCert -create
``` 
This lets LetsencryptCert create an account.

## Obtaining certificate (test mode)
After executing the tool for the first time and having created an account, you can obtain a certificate from LetsEncrypt by running the tool:

```
LetsencryptCert
``` 

Your webserver has to serve the token to LetsEncrypt.

## Obtaining certificate 

To get a secure certificate, you have to set the command line argument ```-prod```:

```
LetsencryptCert -prod
```

## cron job running every day:

```
sudo ~/.dotnet/tools/uwebcert -prod
``` 
