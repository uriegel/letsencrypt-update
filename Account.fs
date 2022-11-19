module Account 

open System.IO
open System.Text.Json
open FSharpTools

open Parameters
open Letsencryptcert
open Certes
open Certes.Acme


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
    acmeContext.NewAccount (certRequest.Account, true) |> Async.AwaitTask |> ignore

    let pemKey = acmeContext.AccountKey.ToPem()
    File.WriteAllTextAsync (getAccountFile (), pemKey) |> Async.AwaitTask |> ignore
    printfn "Letsencrypt account created"
}

let delete () =
    printfn "Deleting letsencrypt account"
    // TODO deleteFile => Result => ignore FSharpTools
    File.Delete (getCertFile ())
//     }
//     catch {}
//     try 
//     {
    File.Delete (getAccountFile ())
//     }
//     catch {}
    printfn "Letsencrypt account deleted"

