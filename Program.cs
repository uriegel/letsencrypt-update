Console.WriteLine("Bin da");
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

// let deleteAllTokens () = 
//     let filterToken (fileInfo: FileInfo) =
//         fileInfo.Extension |> String.isEmpty
    
//     let deleteToken (fileInfo: FileInfo) =
//         File.Delete fileInfo.FullName

//     getEncryptDirectory ()
//     |> Directory.getFiles
//     |>> Array.filter filterToken
//     |>> Array.iter deleteToken
    
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

// printfn "Starting letsencrypt certificate handling"
// match Parameters.get () with
// | { Value.Mode = Create } -> Account.create () |> Async.RunSynchronously
// | _ when not <| Certificate.checkValidationTime () -> 
//     perform () 
//     |> printError
//     |> Async.RunSynchronously
// | _ -> printfn "No further action needed"

// deleteAllTokens ()
// |> throw

// printfn "Letsencrypt certificate handling finished"