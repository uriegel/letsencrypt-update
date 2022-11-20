module Certificate

open Certes
open Certes.Acme
open System.IO

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
    let pfxBuilder = cert.ToPfx privateKey
//     TODO passwd in /etc/letsencrypt-uweb var passwd = "";
//    let pfx = pfxBuilder.Build (certInfo.Data.CommonName, passwd)
    printfn "Saving certificate" 

    // let certificateFile = Path.Combine(encryptDirectory, $"certificate{(staging ? "-staging" : "")}.pfx");

    // File.WriteAllBytes(certificateFile, pfx);
    // TODO error handling

    return Ok ()
}


