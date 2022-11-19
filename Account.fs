module Account 

open System.IO
open System.Text.Json
open FSharpTools

open Parameters
open Letsencryptcert
open Certes
open Certes.Acme
open FSharpTools.Directory
open System.Runtime.CompilerServices
open Certes.Acme.Resource


let private options = JsonSerializerOptions (PropertyNameCaseInsensitive = true)

let readRequest requestFile =
    use file = File.OpenRead requestFile
    JsonSerializer.Deserialize<CertRequest> (file, options)

let create () = async {
    printfn "Creating letsencrypt account"

    let certRequest = readRequest "cert.json"
    if getEncryptDirectory () |> Directory.existsDirectory |> not then 
        getEncryptDirectory () 
        |> Directory.create 
        |> Result.throw 
        |> ignore

    File.Copy ("cert.json", getCertFile (), true)

    let server = 
        if (Parameters.get()).Staging then 
            WellKnownServers.LetsEncryptStagingV2 
        else 
            WellKnownServers.LetsEncryptV2

    let acmeContext = AcmeContext server
    do! acmeContext.NewAccount (certRequest.Account, true) 
        |> Async.AwaitTask 
        |> Async.Ignore

    let pemKey = acmeContext.AccountKey.ToPem()
    File.WriteAllTextAsync (getAccountFile (), pemKey) |> Async.AwaitTask |> ignore
    printfn "Letsencrypt account created"
}

// TODO to FSharpTools
let asyncMap f x = async {
    let! v = x
    return f v
}

let asyncSideEffect f x = async {
    let! a = x
    do! f a
    return a
}

let get () = 
    printfn "Reading letsencrypt account"

    let keyFromPem pem = KeyFactory.FromPem pem

    let readPem () = 
        File.ReadAllTextAsync (getAccountFile ()) 
        |> Async.AwaitTask

    let openAccount (context: AcmeContext) = 
        context.Account () 
            |> Async.AwaitTask
            |> Async.Ignore

    let getAccount (accountKey: IKey) = 
        let result = AcmeContext ((if (get()).Staging then WellKnownServers.LetsEncryptStagingV2 else WellKnownServers.LetsEncryptV2), accountKey)
        printfn "Letsencrypt account read"
        result

    readPem ()
    |> asyncMap keyFromPem
    |> asyncMap getAccount
    |> asyncSideEffect openAccount

let delete () =
    printfn "Deleting letsencrypt account"
    if existsFile <| getCertFile () then 
        File.Delete (getCertFile ())
    if existsFile <| getAccountFile () then 
        File.Delete (getAccountFile ())
    printfn "Letsencrypt account deleted"

