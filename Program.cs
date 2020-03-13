using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Newtonsoft.Json;

namespace UwebServerCert
{
    class Program
    {
        const string accountFile = "/etc/letsencrypt-uweb/access.pem";

        static async Task CreateAccountAsync(bool staging)
        {
            Console.WriteLine("Creating letsencrypt account");

            using var file = File.OpenText("cert.json");
            var serializer = new JsonSerializer();
            var certRequest = serializer.Deserialize(file, typeof(CertRequest)) as CertRequest;
            acmeContext = new AcmeContext(staging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2);
            account = await acmeContext.NewAccount(certRequest.account, true);
            var pemKey = acmeContext.AccountKey.ToPem();
            var fi = new FileInfo(accountFile);
            Directory.CreateDirectory(fi.DirectoryName);
            await File.WriteAllTextAsync(accountFile, pemKey);            
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
                // if (deleteAccount)
                //     Console.WriteLine("Deleting letsencrypt account");
                if (createAccount)
                    await CreateAccountAsync(staging);


                // if (File.Exists(accountFile))
                // {

                // }
                // else
                //     CreateAccount(args[1], staging);
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
        static AcmeContext acmeContext;
        static IAccountContext account;
    }
}
