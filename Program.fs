open Parameters

printfn "Starting letsencrypt certificate handling"
match Parameters.get () with
| { Value.Mode = Create } -> Account.create () |> Async.RunSynchronously
| { Value.Mode = Delete } -> Account.delete ()
| _                       -> ()
printfn "Letsencrypt certificate handling finished"

// using System.Security.Cryptography.X509Certificates;
// using Certes;
// using Certes.Acme;




// string certRequestFile;
// AcmeContext acmeContext = null;
// IAccountContext account;
// string accountFile;

//     Console.WriteLine(staging ? "Staging" : "!!! P R O D U C T I V E !!!");

//     var certificateFile = Path.Combine(encryptDirectory, $"certificate{(staging ? "-staging" : "")}.pfx");

//     CertRequest certRequest = null;
//     if (deleteAccount)
//     {
//         DeleteAccount();
//         return;
//     }
//     else if (createAccount)
//         certRequest = await CreateAccountAsync(staging);
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

//     Console.WriteLine($"Registering domains: {String.Join(", ", certRequest.Domains)}"); 

//     var order = await acmeContext.NewOrder(certRequest.Domains);
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
//     TODO passwd in cert.json var passwd = "uriegel";
//     var pfx = pfxBuilder.Build(certRequest.Data.CommonName, passwd);
//     Console.WriteLine($"Saving certificate"); 
//     File.WriteAllBytes(certificateFile, pfx);
// }
// catch (Exception e)
// {
//     Console.Error.WriteLine($"Exception: {e}");
// }



// async Task<CertRequest> ReadAccountAsync(bool staging)
// {
//     Console.WriteLine("Reading letsencrypt account");
//     var pemKey = await File.ReadAllTextAsync(accountFile);
//     var accountKey = KeyFactory.FromPem(pemKey);
//     acmeContext = new AcmeContext(staging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2, accountKey);
//     account = await acmeContext.Account();                 
//     Console.WriteLine("Letsencrypt account read");
//     return ReadRequest(certRequestFile);
// }

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

