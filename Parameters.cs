// open Certes.Acme
// open Certes.Acme.Resource
// open System.IO
using System.Security.Cryptography;
using CsTools.Extensions;

using static System.Console;
using static CsTools.Functional.Memoization;

record Parameters(
    bool Staging,
    OperationMode Mode
) {
    public static Func<Parameters> Get { get; }
        = Memoize(Init);

    public static Func<string> GetEncryptDirectory { get; }
        = Memoize(InitEncryptDirectory);
    static Parameters Init()
        => Environment.GetCommandLineArgs()
            .Pipe(args => new Parameters(
                args
                    .Contains("-prod")
                    .SideEffect(n => WriteLine(n ? "!!! P R O D U C T I V E !!!" : "Staging"))
                    != true,
                (args.Contains("-create"), args.Contains("-del")) switch
                {
                    (true, false) => OperationMode.Create,
                    (false, true) => OperationMode.Delete,
                    _ => OperationMode.Operate
                })
                    .SideEffect(WriteLine)
            );

    public static string InitEncryptDirectory()
        => Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .AppendPath("letsencrypt-uweb");



    // string? GetEnvironmentVariableWithLogging(this string key)
    //     => key
    //         .GetEnvironmentVariable()
    //         ?.SideEffect(v => WriteLine($"Reading environment {key}: {v}"));

    // let get = 
    //     let get () = 

    //         {
    //         }
    //     memoizeSingle get

    // let getCertFile = 
    //     let getCertFile () = 
    //         getEncryptDirectory ()
    //         |> Directory.attachSubPath "cert.json"
    //     memoizeSingle getCertFile

    // let getAccountFile = 
    //     let getName () = if (get()).Staging then "account-staging.pem" else "account.pem"
    //     let getAccountFile () =
    //         getEncryptDirectory ()
    //         |> Directory.attachSubPath (getName ())

    //     memoizeSingle getAccountFile

    // let getAcmeUri = 
    //     let getAcmeUri () = 
    //         if (get()).Staging 
    //             then WellKnownServers.LetsEncryptStagingV2 
    //         else WellKnownServers.LetsEncryptV2    
    //     memoizeSingle getAcmeUri

    // let getPfxPassword = 
    //     let getPfxPassword () = 
    //         let readAllText path = File.ReadAllText path

    //         if OperatingSystem.IsLinux () 
    //             then 
    //                 "/etc" 
    //             else 
    //                 System.Environment.GetFolderPath System.Environment.SpecialFolder.CommonApplicationData
    //                 |> Directory.attachSubPath "LetsencryptUweb"
    //         |> Directory.attachSubPath "letsencrypt-uweb"
    //         |> readAllText
    //         |> String.trim
    //     memoizeSingle getPfxPassword

    // let getPfxFile = 
    //     let getPfxFile () = 
    //         getEncryptDirectory ()
    //         |> Directory.attachSubPath (if (get()).Staging then "certificate-staging.pfx" else "certificate.pfx")
    //     memoizeSingle getPfxFile
}