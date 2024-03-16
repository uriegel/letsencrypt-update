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

    public static Func<string> GetPfxFile { get; }
        = Memoize(InitGetPfxFile);

    public static Func<string> GetPfxPassword { get; }
        = Memoize(InitGetPfxPassword);

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

    static string InitEncryptDirectory()
        => Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .AppendPath("letsencrypt-uweb");

    static string InitGetPfxFile()
        => GetEncryptDirectory()
            .AppendPath(Get().Staging ? "certificate-staging.pfx" : "certificate.pfx");

    static string InitGetPfxPassword()
        => Get()
            .Staging
                ? "/etc"
                : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            .AppendPath("LetsencryptUweb")
            .ReadAllTextFromFilePath()
            ?.Trim()
            ?? "".SideEffect(_ => WriteLine("!!!NO PASSWORD!!"));

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

}