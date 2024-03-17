// open Certes.Acme
using System.Text.Json;
using Certes;
using CsTools;
using CsTools.Extensions;
using CsTools.Functional;

using static System.Console;
using static CsTools.Functional.Memoization;

static class Account
{
    public static async Task Create()
    {
        WriteLine("Creating letsencrypt account");
        var certRequest = ReadRequest();
        if (certRequest == null)
        {
            WriteLine("You have to create a cert request json file, see https://www.nuget.org/packages/LetsencryptCert/");
            return;
        }
//     if getEncryptDirectory () |> Directory.existsDirectory |> not then 
//         getEncryptDirectory () 
//         |> Directory.create 
//         |> Result.throw 
//         |> ignore

//     File.Copy ("cert.json", getCertFile (), true)

//     let server = 
//         if (Parameters.get()).Staging then 
//             WellKnownServers.LetsEncryptStagingV2 
//         else 
//             WellKnownServers.LetsEncryptV2

//     let acmeContext = AcmeContext server
//     do! acmeContext.NewAccount (certRequest.Account, true) 
//         |> Async.AwaitTask 
//         |> Async.Ignore

//     let pemKey = acmeContext.AccountKey.ToPem()
//     File.WriteAllTextAsync (getAccountFile (), pemKey) |> Async.AwaitTask |> ignore
//     printfn "Letsencrypt account created"
// }
    }

    public static AsyncResult<AcmeContext, Unit> Get()
    {
        WriteLine("Reading letsencrypt account");

        return (Parameters
            .GetAccountFile()
            .ReadAllTextFromFilePath()
            ?.Pipe(p => KeyFactory.FromPem(p))
            ?.Pipe(k => new AcmeContext(Parameters.GetAcmeUri(), k))
            ?.SideEffect(_ => WriteLine("Letsencrypt account read")))
            .FromNullable()
            .SideEffectWhenOkAsync(a => a.Account())
            .ToAsyncResult();
    }

    public static Func<CertRequest?> ReadRequest { get; }
        = MemoizeMaybe(InitReadRequest);

    static CertRequest? InitReadRequest()
        => Parameters
                .GetCertFile()
                ?.OpenFile()
                ?.Use(f => JsonSerializer.Deserialize<CertRequest>(f, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
}



