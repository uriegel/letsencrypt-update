module Certificate

open Certes
open Certes.Acme
open FSharpTools
open FSharpTools.Functional
open System
open System.IO
open System.Security.Cryptography.X509Certificates

open Option

let order (order: IOrderContext): Async<Result<Unit, string>> = async {    
    printfn "Ordering certificate" 
    
    let certInfo = Account.readRequest <| Parameters.getCertFile ()
    let privateKey = KeyFactory.NewKey KeyAlgorithm.ES256
    let! cert = order.Generate (CsrInfo (
            CountryName = certInfo.Data.CountryName, 
            State = certInfo.Data.State,
            Locality = certInfo.Data.Locality, 
            Organization = certInfo.Data.Organization,
            OrganizationUnit = certInfo.Data.OrganizationUnit,
            CommonName = certInfo.Data.CommonName
        ), privateKey) |> Async.AwaitTask

    printfn "Creating certificate" 
    let certPem = cert.ToPem ()
    let certKey = privateKey.ToPem ()

    let x509 = X509Certificate2.CreateFromPem (certPem.ToCharArray (), certKey.ToCharArray ())
    let pfxBytes = x509.Export (X509ContentType.Pfx, Parameters.getPfxPassword ())
    printfn "Saving certificate" 
    File.WriteAllBytes (Parameters.getPfxFile (), pfxBytes)
    return Ok ()
}

let checkValidationTime () =
    let printValidationDate (certificate: X509Certificate2) = 
        printfn "Certificate expires:%O" certificate.NotAfter

    let openPfx () = 
        match Parameters.getPfxFile () |> Directory.existsFile with
        | true -> Some (new X509Certificate2 (Parameters.getPfxFile (), Parameters.getPfxPassword ())
                        |> sideEffect printValidationDate)
        | false -> None
    
    let checkValidationTime (certificate: X509Certificate2) = 
        certificate.NotAfter > DateTime.Now + TimeSpan.FromDays 30

    openPfx ()
    |>> checkValidationTime
    |> defaultValue false  



