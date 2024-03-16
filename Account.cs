// open Certes
// open Certes.Acme
// open FSharpTools
// open FSharpTools.Functional
// open System.IO
// open System.Text.Json

// open Letsencryptcert
// open Parameters

static class Account
{
    public static void Create()
    {
        
    }
}
// let private options = JsonSerializerOptions (PropertyNameCaseInsensitive = true)

// let readRequest =
//     let readRequest requestFile =
//         use file = File.OpenRead requestFile
//         JsonSerializer.Deserialize<CertRequest> (file, options)
//     memoize readRequest

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

// let get () = 
//     printfn "Reading letsencrypt account"

//     let keyFromPem pem = KeyFactory.FromPem pem

//     let readPem () = 
//         File.ReadAllTextAsync (getAccountFile ()) 
//         |> Async.AwaitTask

//     let openAccount (context: AcmeContext) = 
//         context.Account () 
//             |> Async.AwaitTask
//             |> Async.Ignore

//     let getAccount (accountKey: IKey) = 
//         let result = AcmeContext (Parameters.getAcmeUri (), accountKey)
//         printfn "Letsencrypt account read"
//         result

//     readPem ()
//     |> Async.map keyFromPem
//     |> Async.map getAccount
//     |> Async.sideEffect openAccount

