module Authorization

open Certes;
open Certes.Acme
open FSharp.Control
open FSharpTools
open FSharpTools.Functional
open System.IO

open Parameters



// TODO to FSharpTools
let asyncBind f x = async {
    let! v = x
    return!f v
}
let inline (>>=) x binder = asyncBind binder x
let inline (>=>) f1 f2 x = f1 x >>= f2

let validateAll: IOrderContext->Async<Unit> =

    // Building blocks

    let validate (auth: IAuthorizationContext) =

        let writeKeyTokenFile (challenge: IChallengeContext) = 
            File.WriteAllTextAsync (Directory.combinePathes [| 
                    getEncryptDirectory () 
                    challenge.Token |> sideEffect (printfn "Validating LetsEncrypt token: %s")
                |], challenge.KeyAuthz)
            |> Async.AwaitTask

        auth.Http() 
        |> Async.AwaitTask
        >>= writeKeyTokenFile

    let getAuthorizations (order: IOrderContext) = 
        order.Authorizations () |> Async.AwaitTask
    
    let iterAuthorizations (authorizations: IAuthorizationContext seq) = async {
        do! authorizations
            |> AsyncSeq.ofSeq
            |> AsyncSeq.iterAsync validate
    }

    // Function composition

    getAuthorizations >=> iterAuthorizations




