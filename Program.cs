using System.Security.Cryptography;
using CsTools;
using CsTools.Extensions;
using Org.BouncyCastle.Crypto.Digests;
using static System.Console;

// open Certes.Acme
// open FSharpTools
// open System
// open System.IO

// open Parameters
// open Result

// let retrieveCert (order: IOrderContext) result = async { 
//     return! 
//         match result with
//         | Ok _ -> Certificate.order order
//         | Error err -> result |> Async.toAsync
// }

// open Async

// let performOrder (order: IOrderContext) = 
//     Authorization.validateAll order
//     >>= retrieveCert order

// let printError result = async {
//     match! result with
//     | Ok _ -> return ()
//     | Error err -> err |> printfn ("An error has occurred: %s")
// }

WriteLine("Starting letsencrypt certificate handling");

await (Parameters.Get() switch
{
    { Staging: true, Mode: OperationMode.Create } => 1.ToAsync().SideEffect(async _ => Account.Create()),
    _ when !Certificate.CheckValidationTime() => 2.ToAsync().SideEffect(_ => Perform()),
    _ => 3.ToAsync().SideEffect(async _ => WriteLine("No further action needed"))
})
    .SideEffect(async _ => DeleteAllTokens());

WriteLine("Letsencrypt certificate handling finished");

static void DeleteAllTokens() 
    => Parameters
        .GetEncryptDirectory()
        .GetFiles()
        .Where(n => !string.IsNullOrEmpty(n.Extension))
        .ForEach(n => File.Delete(n.FullName));

static async Task Perform()
{
//     let! acme = Account.get ()

//     let getCertData () = 
//         getCertFile ()
//         |> Account.readRequest 

//     (getCertData ()).Domains 
//     |> String.joinStr ", "
//     |> printfn "Registering domains: %s"  

//     return! 
//         acme.NewOrder (getCertData ()).Domains 
//         |> Async.AwaitTask
//         >>= performOrder

}
