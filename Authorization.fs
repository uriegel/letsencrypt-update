module Authorization

open Certes;
open Certes.Acme
open Certes.Acme.Resource
open FSharp.Control
open FSharpTools
open FSharpTools.AsyncResult
open FSharpTools.Functional
open System
open System.IO

open Parameters
open Async    

let validateAll: IOrderContext->Async<Result<unit, string>> =

    // Building blocks

    let validate (state: Result<Unit, string>) (auth: IAuthorizationContext) =

        let printChallengeResult (result: Resource.Challenge) = async {
            printfn "Challenge: %O, %O, %O" result.Error result.Status result.Validated 
        } 

        let matchChallengeResult (result: Async<Resource.Challenge>) = async {
            let! status = result
            return 
                match Option.ofNullable status.Status with
                | Some ChallengeStatus.Invalid -> 
                    printfn "Could not validate LetsEncrypt token: %s" status.Token
                    Error "not valid"
                | Some ChallengeStatus.Valid -> Ok ()
                | _ -> Error "unknown"
        }

        let validateChallenge (challenge: IChallengeContext) = 
            challenge.Validate () 
            |> Async.AwaitTask
            |> Async.sideEffect printChallengeResult
            |> matchChallengeResult

        let writeKeyTokenFile (challenge: IChallengeContext) = 
            File.WriteAllTextAsync (Directory.combinePathes [| 
                    getEncryptDirectory () 
                    challenge.Token |> Functional.sideEffect (printfn "Validating LetsEncrypt token: %s")
                |], challenge.KeyAuthz)
            |> Async.AwaitTask

        let validate (challenge: Async<IChallengeContext>) = 
            let validate () = 
                challenge 
                |> Async.sideEffect writeKeyTokenFile 
                |> Async.bind validateChallenge
            validate
            |> repeatOnError (TimeSpan.FromSeconds 3) 7            

        match state with
        | Ok _ -> 
            auth.Http() 
            |> Async.AwaitTask
            |> validate
        | Error err -> state |> Async.toAsync

    let getAuthorizations (order: IOrderContext) = 
        order.Authorizations () |> Async.AwaitTask

    let iterAuthorizations (authorizations: IAuthorizationContext seq) = async {
        return! authorizations
            |> AsyncSeq.ofSeq
            |> AsyncSeq.foldAsync validate (Ok ())
    }

    // Function composition

    getAuthorizations >=> iterAuthorizations




