module Certificate

open Certes
open Certes.Acme
open System.IO
open System.Security.Cryptography.X509Certificates

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
    let pfxBytes = x509.Export (X509ContentType.Pfx, "cvbcvbcvbcvbcbxcb xcvb cxb xcb xcb xcvb xcvb xcb xcvb xcvb xcb xcvb xcvb xcvb cvxb xc")
    let zertifikat = "/home/uwe/certificate.pfx"
    File.WriteAllBytes (zertifikat, pfxBytes)

    let pfxBuilder = cert.ToPfx 
//     TODO passwd in /etc/letsencrypt-uweb var passwd = "";

    printfn "Saving certificate" 

    // let certificateFile = Path.Combine(encryptDirectory, $"certificate{(staging ? "-staging" : "")}.pfx");

    // File.WriteAllBytes(certificateFile, pfx);
    // TODO error handling

    return Ok ()
}


