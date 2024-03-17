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
    { Staging: true, Mode: OperationMode.Create } => 1.ToAsync().SideEffectAsync(_ => Account.Create()),
    _ when !Certificate.CheckValidationTime() => 2.ToAsync().SideEffectAwait( _ => Perform()),
    _ => 3.ToAsync().SideEffectAsync(_ => WriteLine("No further action needed"))
})
    .SideEffectAsync( _ => DeleteAllTokens());

WriteLine("Letsencrypt certificate handling finished");

static void DeleteAllTokens() 
    => Parameters
        .GetEncryptDirectory()
        .GetFiles()
        .Where(n => !string.IsNullOrEmpty(n.Extension))
        .ForEach(n => File.Delete(n.FullName));

static Task Perform()
{
    Certes.AcmeContext? c= null;
    var x = c?.NewOrder(Account.ReadRequest()?.Domains).GetOrNull(); 
    var xx = x.GetOrNull();

    var t = Account
                .Get()
                .SelectManyMaybe(c => xx);


    // var t =  Account
    //     .Get()
    //     .SelectManyMaybe(c => c?.NewOrder(Account.ReadRequest()?.Domains))

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
