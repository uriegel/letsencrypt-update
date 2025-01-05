# letsencrypt-update
LetsEncrypt bot to automatically create an LetsEncrypt certificate for web server (especially uriegel/home-server)

## Prerequisites

``` 
sudo dnf install openssl-devel
```

## Executing the tool for the first time

Copy ```conf.json``` to directory: ```~/.config/letsencrypt-cert``` with following content:

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
## cron job running every day:

```
0 4 * * * /home/uwe/home_server/letsencrypt-cert > /home/uwe/logs/letsencrypt-cert.log 2>&1
``` 
Webserver has to be restarted in order to take renewed certificate
