// open Certes
// open Certes.Acme
using System.Security.Cryptography.X509Certificates;
using CsTools.Extensions;

using static System.Console;

static class Certificate
{
    public static bool CheckValidationTime()
        => Parameters
            .GetPfxFile()
            .Pipe(p =>
                File.Exists(p)
                ? p
                : null)
            ?.Pipe(p => new X509Certificate2(p, Parameters.GetPfxPassword())
                            .SideEffect(c => WriteLine($"Certificate expires: {c.NotAfter}")))
            ?.Pipe(c => c.NotAfter > DateTime.Now + TimeSpan.FromDays(30)) == true;
    

// let order (order: IOrderContext): Async<Result<Unit, string>> = async {    
//     printfn "Ordering certificate" 
    
//     let certInfo = Account.readRequest <| Parameters.getCertFile ()
//     let privateKey = KeyFactory.NewKey KeyAlgorithm.ES256
//     let! cert = order.Generate (CsrInfo (
//             CountryName = certInfo.Data.CountryName, 
//             State = certInfo.Data.State,
//             Locality = certInfo.Data.Locality, 
//             Organization = certInfo.Data.Organization,
//             OrganizationUnit = certInfo.Data.OrganizationUnit,
//             CommonName = certInfo.Data.CommonName
//         ), privateKey) |> Async.AwaitTask

//     printfn "Creating certificate" 
//     let certPem = cert.ToPem ()
//     let certKey = privateKey.ToPem ()

//     let x509 = X509Certificate2.CreateFromPem (certPem.ToCharArray (), certKey.ToCharArray ())
//     let pfxBytes = x509.Export (X509ContentType.Pfx, Parameters.getPfxPassword ())
//     printfn "Saving certificate" 
//     File.WriteAllBytes (Parameters.getPfxFile (), pfxBytes)
//     return Ok ()
}



