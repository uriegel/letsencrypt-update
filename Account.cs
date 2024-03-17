// open Certes.Acme
using System.Text.Json;
using Certes;
using CsTools.Extensions;
using CsTools.Functional;
using static System.Console;

using static CsTools.Functional.Memoization;

static class Account
{
    public static void Create()
    {
        
    }

    public static Task<AcmeContext?> Get()
    {
        WriteLine("Reading letsencrypt account");

        return Parameters
            .GetAccountFile()
            .ReadAllTextFromFilePath()
            ?.Pipe(p => KeyFactory.FromPem(p))
            ?.Pipe(k => new AcmeContext(Parameters.GetAcmeUri(), k))
            ?.SideEffect(_ => WriteLine("Letsencrypt account read"))
            .SideEffectAsync(a => a.Account())
            .GetOrNull()
            ?? Task.FromResult(null as AcmeContext);
    }

    public static Func<CertRequest?> ReadRequest { get; }
        = MemoizeMaybe(InitReadRequest);

    static CertRequest? InitReadRequest()
        => Parameters
                .GetCertFile()
                .OpenFile()
                .Use(f => JsonSerializer.Deserialize<CertRequest>(f, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
}

// let create () = async {
//     printfn "Creating letsencrypt account"

//     let certRequest = readRequest "cert.json"
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


