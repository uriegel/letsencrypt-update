using Microsoft.VisualBasic;

static class Extensions
{
    public static FileInfo[] GetFiles(this string path)
        => new DirectoryInfo(path).GetFiles();

    // TODO
    public static Task<T?> GetOrNull<T>(this Task<T?>? t)
        where T: class
        => t as Task<T?> ?? Task.FromResult<T?>(null);

    public static async Task<TR> SelectMany<T, TR>(this Task<T> t, Func<T, Task<TR>> selector)
        => await selector(await t);

    public static async Task<TR?> SelectManyMaybe<T, TR>(this Task<T?> t, Func<T?, Task<TR?>> selector)
        where T: class
        where TR: class
    {
        var val = await t;
        var ret = selector(val);
        return await ret;            
    }
}