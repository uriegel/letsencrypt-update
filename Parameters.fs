module Parameters

open System
open FSharpTools
open FSharpTools.Functional

type Mode =
| Create
| Operate
| Delete

type Value = {
    Staging: bool
    Mode: Mode
}

/// Evaluating command args
/// 
/// the following arguments are valid:
/// 
/// * ```-prod``` productive, without: staging (test)
/// * ```-create``` creates file cert.json, then exits. Setup step
/// * ```-del``` delete account
/// 
///**Returns**
/// ```Value``` object
let get = 
    let get () = 
        
        let printProductive b =
            if b then "!!! P R O D U C T I V E !!!" else "Staging" 
            |> printfn "%s"

        let args = Environment.GetCommandLineArgs ()
        {
            Staging = 
                args 
                |> Array.contains "-prod" 
                |> sideEffect printProductive 
                |> not
            Mode = 
                match args |> Array.contains "-create", args |> Array.contains "-del" with
                | true, false -> Create
                | false, true -> Delete
                | _           -> Operate
        }
    memoizeSingle get

let getEncryptDirectory =
    let getEncryptDirectory () =
        Environment.GetFolderPath Environment.SpecialFolder.ApplicationData
        |> Directory.attachSubPath "letsencrypt-uweb"
    memoizeSingle getEncryptDirectory

let getCertFile = 
    let getCertFile () = 
        getEncryptDirectory ()
        |> Directory.attachSubPath "cert.json"
    memoizeSingle getCertFile
    
let getAccountFile = 
    let getAccountFile () =
        getEncryptDirectory ()
        |> Directory.attachSubPath "accounts.pem"

    memoizeSingle getAccountFile
