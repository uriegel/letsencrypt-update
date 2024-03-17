static class Extensions
{
    public static FileInfo[] GetFiles(this string path)
        => new DirectoryInfo(path).GetFiles();

    public static string? CheckIfFileExists(this string path)
        => File.Exists(path)
            ? path
            : null;
}    
 