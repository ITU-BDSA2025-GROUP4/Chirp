namespace SimpleDB;

using System.Collections.Concurrent;
using CsvHelper.Configuration;


public enum DatabaseType
{
    SQL, CSV
}
public static class DatabaseSessionRegistry<T> where T : class
{
    private static readonly ConcurrentDictionary<string, IDatabaseRepository<T>> Sessions =
        new(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

    private static string KeyForPath(string path) =>
        Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);

    public static IDatabaseRepository<T> OpenFile(DatabaseType type, string path, CsvConfiguration? cfg = null) =>
        Sessions.GetOrAdd(KeyForPath(path), _ => {
                switch(type)
                {
                    case DatabaseType.CSV:
                        return new CsvDatabase<T>(path, cfg);
                    case DatabaseType.SQL:
                        if(cfg != null) throw new ArgumentException("Got SQL database with CSV config");
                        return new SQLiteDatabase<T>(path);
                }
                // C# won't compile without this despite all cases in switch being covered
                return null;
    });

    public static bool CloseFile(string path) =>
        Sessions.TryRemove(KeyForPath(path), out _);

    public static IDatabaseRepository<T> OpenInMemory(string sessionId, TextReader seed, CsvConfiguration? cfg = null) =>
        Sessions.GetOrAdd("mem:" + sessionId, _ => new CsvDatabase<T>(seed, cfg));

    public static IDatabaseRepository<T> OpenInMemory(string sessionId) =>
        Sessions.GetOrAdd("mem:" + sessionId, _ => new SQLiteDatabase<T>());

    public static bool Close(string sessionId) =>
        Sessions.TryRemove(sessionId, out _);
}