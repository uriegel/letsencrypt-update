open Certes.Acme
open FSharpTools
open Parameters
open Async

let retrieveCert (order: IOrderContext) result = async { 
    return! 
        match result with
        | Ok _ -> Certificate.order order
        | Error err -> result |> Async.toAsync
}

open Authorization
let performOrder (order: IOrderContext) = 
    Authorization.validateAll order
    >>= retrieveCert order

let perform () = async {
    let! acme = Account.get ()

    let getCertData () = 
        getCertFile ()
        |> Account.readRequest 

    (getCertData ()).Domains 
    |> String.joinStr ", "
    |> printfn "Registering domains: %s"  

    return! 
        acme.NewOrder (getCertData ()).Domains 
        |> Async.AwaitTask
        >>= performOrder
}

let printError result = async {
    match! result with
    | Ok _ -> return ()
    | Error err -> err |> printfn ("An error has occurred: %s")
}

printfn "Starting letsencrypt certificate handling"
match Parameters.get () with
| { Value.Mode = Create } -> Account.create () |> Async.RunSynchronously
| _                       -> 
    perform () 
    |> printError
    |> Async.RunSynchronously

// TODO delete all token files: check if file contains a dot then it is not a token
printfn "Letsencrypt certificate handling finished"

// TODO check certificate if too old
//     var certificateFile = Path.Combine(encryptDirectory, $"certificate{(staging ? "-staging" : "")}.pfx");
//     CertRequest certRequest = null;
//     else  
//     {
//         if (File.Exists(certificateFile))
//         {
//             var certificate = new X509Certificate2(certificateFile, "uriegel");

//             Console.WriteLine($"Certificate expires: {certificate.NotAfter}");
//             if (certificate.NotAfter > DateTime.Now + TimeSpan.FromDays(30))
//             {
//                 Console.WriteLine("No further action needed");
//                 return;
//             }                    
//         }
//         certRequest = await ReadAccountAsync(staging);
//     }





