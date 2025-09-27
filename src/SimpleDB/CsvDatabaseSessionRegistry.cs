using System.Collections.Concurrent;
using CsvHelper.Configuration;

namespace SimpleDB;

public static class CsvDatabaseSessionRegistry<T>
{
    private static readonly ConcurrentDictionary<string, CsvDatabase<T>> Sessions =
        new(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

    private static string KeyForPath(string path) =>
        Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);

    public static CsvDatabase<T> OpenFile(string path, CsvConfiguration? cfg = null) =>
        Sessions.GetOrAdd(KeyForPath(path), _ => new CsvDatabase<T>(path, cfg));

    public static bool CloseFile(string path) =>
        Sessions.TryRemove(KeyForPath(path), out _);

    public static CsvDatabase<T> OpenInMemory(string sessionId, TextReader seed, CsvConfiguration? cfg = null) =>
        Sessions.GetOrAdd("mem:" + sessionId, _ => new CsvDatabase<T>(seed, cfg));

    public static bool Close(string sessionId) =>
        Sessions.TryRemove(sessionId, out _);
}