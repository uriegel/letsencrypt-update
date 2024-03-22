using CsTools.HttpRequest;

using static System.Console;
using static CsTools.HttpRequest.Core;

static class HttpChecker
{
    public static async Task<bool> Check(string url)
    {
        try
        {
            var msg = await Request.RunAsync(DefaultSettings with
                {
                    Method = HttpMethod.Get,
                    BaseUrl = url, //"http://192.168.178.74:8080",
                    Url = "/.well-known/acme-challenge/check"
                });
            return await msg.Content.ReadAsStringAsync() == "checked";
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre && hre.HttpRequestError == HttpRequestError.ConnectionError)
        {
            WriteLine("HTTP server not running!");
            return false;
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre && hre.HttpRequestError == HttpRequestError.NameResolutionError)
        {
            WriteLine($"Unknown domain {url}!");
            return false;
        }
        catch (Exception e)
        {
            WriteLine(e);
            return false;
        }
    }
}