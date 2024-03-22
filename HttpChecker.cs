using CsTools.HttpRequest;

using static System.Console;
using static CsTools.HttpRequest.Core;

static class HttpChecker
{
    public static async Task Check(string domain)
    {
        try
        {
            var msg = await Request.RunAsync(DefaultSettings with
                {
                    Method = HttpMethod.Get,
                    BaseUrl = $"HTTP://{domain}",
                    Url = "/.well-known/acme-challenge/check"
                });
            if (await msg.Content.ReadAsStringAsync() != "checked")
                throw new HttpNotReadyException();
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre && hre.HttpRequestError == HttpRequestError.ConnectionError)
        {
            WriteLine("HTTP server not running!");
            throw new Exception();
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre && hre.HttpRequestError == HttpRequestError.NameResolutionError)
        {
            WriteLine($"Unknown domain: {domain}!");
            throw new Exception();
        }
        catch (HttpNotReadyException)
        {
            WriteLine("HTTP server not ready for Lets Encrypt!");
            throw;
        }
        catch (Exception e)
        {
            WriteLine(e);
            throw new Exception();
        }
    }
}