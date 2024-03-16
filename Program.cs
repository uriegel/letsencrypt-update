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

// let perform () = async {
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
// }

// let printError result = async {
//     match! result with
//     | Ok _ -> return ()
//     | Error err -> err |> printfn ("An error has occurred: %s")
// }

WriteLine("Starting letsencrypt certificate handling");

(Parameters.Get() switch
{
    { Staging: true, Mode: OperationMode.Create } => 1.SideEffect(_ => Account.Create()),
    // | _ when not <| Certificate.checkValidationTime () -> 
    //     perform () 
    //     |> printError
    _ => 3.SideEffect(_ => WriteLine("No further action needed"))
})
    .SideEffect(_ => DeleteAllTokens());

WriteLine("Letsencrypt certificate handling finished");

static void DeleteAllTokens() 
    => Parameters
        .GetEncryptDirectory()
        .GetFiles()
        .Where(n => !string.IsNullOrEmpty(n.Extension))
        .ForEach(n => File.Delete(n.FullName));

