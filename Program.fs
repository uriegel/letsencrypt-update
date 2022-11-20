open Certes.Acme
open FSharpTools
open Parameters
open Async

let retrieveCert (order: IOrderContext) result = async { 
    return! 
        match result with
        | Ok _ -> Certificate.order order
        | Error err -> result |> Async.toAsync
}

open Authorization
let performOrder (order: IOrderContext) = 
    Authorization.validateAll order
    >>= retrieveCert order

let perform () = async {
    let! acme = Account.get ()

    let getCertData () = 
        getCertFile ()
        |> Account.readRequest 

    (getCertData ()).Domains 
    |> String.joinStr ", "
    |> printfn "Registering domains: %s"  

    return! 
        acme.NewOrder (getCertData ()).Domains 
        |> Async.AwaitTask
        >>= performOrder
}

let printError result = async {
    match! result with
    | Ok _ -> return ()
    | Error err -> err |> printfn ("An error has occurred: %s")
}

printfn "Starting letsencrypt certificate handling"
match Parameters.get () with
| { Value.Mode = Create } -> Account.create () |> Async.RunSynchronously
| _                       -> 
    perform () 
    |> printError
    |> Async.RunSynchronously

// TODO delete all token files
printfn "Letsencrypt certificate handling finished"

// using System.Security.Cryptography.X509Certificates;
// using Certes;
// using Certes.Acme;

//     var certificateFile = Path.Combine(encryptDirectory, $"certificate{(staging ? "-staging" : "")}.pfx");
//     CertRequest certRequest = null;
//     else  
//     {
//         if (File.Exists(certificateFile))
//         {
//             var certificate = new X509Certificate2(certificateFile, "uriegel");

//             Console.WriteLine($"Certificate expires: {certificate.NotAfter}");
//             if (certificate.NotAfter > DateTime.Now + TimeSpan.FromDays(30))
//             {
//                 Console.WriteLine("No further action needed");
//                 return;
//             }                    
//         }
//         certRequest = await ReadAccountAsync(staging);
//     }




//     var authorizations = (await order.Authorizations()).ToArray();
//     foreach (var authorization in authorizations)
//         await ValidateAsync(authorization);

//     Console.WriteLine($"Ordering certificate"); 
//     var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
//     var cert = await order.Generate(new CsrInfo
//     {
//         CountryName = certRequest.Data.CountryName,
//         State = certRequest.Data.State,
//         Locality = certRequest.Data.Locality,
//         Organization = certRequest.Data.Organization,
//         OrganizationUnit = certRequest.Data.OrganizationUnit,
//         CommonName = certRequest.Data.CommonName,
//     }, privateKey);

//     Console.WriteLine($"Creating certificate"); 
//     var certPem = cert.ToPem();
//     var pfxBuilder = cert.ToPfx(privateKey);

//     //var passwordFile = Path.Combine(encryptDirectory, $"passwd{(staging ? "-staging" : "")}");
//     //var passwd = Guid.NewGuid().ToString();
//     // File.WriteAllText(passwordFile, passwd);
//     TODO passwd in /etc/letsencrypt-uweb var passwd = "";
//     var pfx = pfxBuilder.Build(certRequest.Data.CommonName, passwd);
//     Console.WriteLine($"Saving certificate"); 
//     File.WriteAllBytes(certificateFile, pfx);



// async Task ValidateAsync(IAuthorizationContext authorization)
// {
//     try 
//     {
//         var httpChallenge = await authorization.Http();
//         var keyAuthz = httpChallenge.KeyAuthz;
//         var token = httpChallenge.Token;
//         Console.WriteLine($"Validating LetsEncrypt token: {token}"); 
//         await File.WriteAllTextAsync(Path.Combine(encryptDirectory, "token"), keyAuthz);

//         while (true)
//         {
//             var challenge = await httpChallenge.Validate();
//             Console.WriteLine($"Challenge: {challenge.Error}, {challenge.Status} {challenge.Validated}"); 
//             if (challenge.Status == Certes.Acme.Resource.ChallengeStatus.Invalid)
//             {
//                 Console.WriteLine($"Could not validate LetsEncrypt token: {token}"); 
//                 throw new Exception("Not valid");
//             }
//             if (challenge.Status == Certes.Acme.Resource.ChallengeStatus.Valid)
//                 break;
//             await Task.Delay(2000);
//         }
//     }
//     finally
//     {
//         try 
//         {
//             File.Delete(Path.Combine(encryptDirectory, "token"));
//         }
//         catch {}
//     }
// }

