module Authorization

open Certes;
open Certes.Acme
open FSharpTools
open FSharpTools.Functional
open System.IO

open Parameters

let validate (auth: IAuthorizationContext) = async {

    let writeKeyTokenFile (challenge: IChallengeContext) = 
        File.WriteAllTextAsync (Directory.combinePathes [| 
                getEncryptDirectory () 
                challenge.Token |> sideEffect (printf "Validating LetsEncrypt token: %s")
            |], challenge.KeyAuthz)
        |> Async.AwaitTask

    return ()
    //let! affe = auth.Http() |> Async.AwaitTask
    
}

let affe (ctx: IAuthorizationContext) = 
    ()

let validateAll (order: IOrderContext) = async {
    let! a = order.Authorizations () |> Async.AwaitTask
    a
    |> Seq.iter affe
}