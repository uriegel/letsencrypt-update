using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UwebServerCert
{
    class Program
    {
        static async Task<CertRequest> CreateAccountAsync(bool staging)
        {
            Console.WriteLine("Creating letsencrypt account");

            var certRequest = ReadRequest("cert.json");
            File.Copy("cert.json", Path.Combine(encryptDirectory, "cert.json"));

            acmeContext = new AcmeContext(staging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2);
            account = await acmeContext.NewAccount(certRequest.Account, true);
            var pemKey = acmeContext.AccountKey.ToPem();
            var fi = new FileInfo(accountFile);
            Directory.CreateDirectory(fi.DirectoryName);
            await File.WriteAllTextAsync(accountFile, pemKey);            
            Console.WriteLine("Letsencrypt account created");
            return certRequest;
        }

        static async Task<CertRequest> ReadAccountAsync(bool staging)
        {
            Console.WriteLine("Reading letsencrypt account");
            var pemKey = await File.ReadAllTextAsync(accountFile);
            var accountKey = KeyFactory.FromPem(pemKey);
            acmeContext = new AcmeContext(staging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2, accountKey);
            account = await acmeContext.Account();                 
            Console.WriteLine("Letsencrypt account read");
            return ReadRequest("cert.json");
        }

        static void DeleteAccount()
        {
            Console.WriteLine("Deleting letsencrypt account");
            try 
            {
                File.Delete(Path.Combine(encryptDirectory, "cert.json"));
            }
            catch {}
            try 
            {
                File.Delete(accountFile);
            }
            catch {}
            try 
            {
                File.Delete(Path.Combine(encryptDirectory, "token"));
            }
            catch {}
            Console.WriteLine("Letsencrypt account deleted");
        }

        static CertRequest ReadRequest(string requestFile)
        {
            using var file = File.OpenText(requestFile);
            var serializer = new JsonSerializer{
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return serializer.Deserialize(file, typeof(CertRequest)) as CertRequest;
        }

        // Parameter: -prod: productive, without: staging (test)
        // Parameter: -del: delete account
        // Parameter: -create: read file cert.json
        static async Task Main(string[] args)
        {
            try 
            {
                Console.WriteLine($"Starting letsencrypt certificate handling");
                
                bool staging = !args.Contains("-prod");
                bool deleteAccount = args.Contains("-del");
                bool createAccount = args.Contains("-create");
                Console.WriteLine(staging ? "Staging" : "!!! P R O D U C T I V E !!!");
                CertRequest certRequest = null;
                if (deleteAccount)
                {
                    DeleteAccount();
                    return;
                }
                else if (createAccount)
                    certRequest = await CreateAccountAsync(staging);
                else    
                    certRequest = await ReadAccountAsync(staging);

                Console.WriteLine($"Registering domains: {String.Join(", ", certRequest.Domains)}"); 
                var order = await acmeContext.NewOrder(certRequest.Domains);

                var autorizations = (await order.Authorizations()).ToArray();
                Console.WriteLine($"Authorizations: {autorizations.Length}"); 
                
                foreach (var autorization in autorizations)
                {
                    var httpChallenge = await autorization.Http();
                    var keyAuthz = httpChallenge.KeyAuthz;
                    var token = httpChallenge.Token;
                    await File.WriteAllTextAsync(Path.Combine(encryptDirectory, "token"), keyAuthz);
                    
                    Console.WriteLine($"Validating LetsEncrypt token"); 
                    var challenge = await httpChallenge.Validate();
                    Console.WriteLine($"Challenge: {challenge.Error}, {challenge.Status} {challenge.Validated}"); 
                    Thread.Sleep(5000);
                    Console.WriteLine($"Validating LetsEncrypt token"); 
                    challenge = await httpChallenge.Validate();
                    Console.WriteLine($"Challenge: {challenge.Error}, {challenge.Status} {challenge.Validated}"); 
                }

                Console.WriteLine($"Ordering certificate"); 
                var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
                var cert = await order.Generate(new CsrInfo
                {
                    CountryName = certRequest.Data.CountryName,
                    State = certRequest.Data.State,
                    Locality = certRequest.Data.Locality,
                    Organization = certRequest.Data.Organization,
                    OrganizationUnit = certRequest.Data.OrganizationUnit,
                    CommonName = certRequest.Data.CommonName,
                }, privateKey);

                Console.WriteLine($"Creating certificate"); 
                var certPem = cert.ToPem();
                var pfxBuilder = cert.ToPfx(privateKey);
                var pfx = pfxBuilder.Build("uriegel.de", "uriegel");
                Console.WriteLine($"Saving certificate"); 
                File.WriteAllBytes(Path.Combine(encryptDirectory, "certificate.pfx"), pfx);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception: {e}");
            }
            finally 
            {
                Console.WriteLine("Letsencrypt certificate handling finished");
            }
        }
        
        static Program()
        {
            accountFile = Path.Combine(encryptDirectory, "access.pem");            
        }

        static AcmeContext acmeContext;
        static IAccountContext account;
        const string encryptDirectory = "/etc/letsencrypt-uweb";
        static readonly string accountFile;
    }
}
