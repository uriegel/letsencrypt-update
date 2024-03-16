static class Extensions
{
    public static FileInfo[] GetFiles(this string path)
        => new DirectoryInfo(path).GetFiles();
}