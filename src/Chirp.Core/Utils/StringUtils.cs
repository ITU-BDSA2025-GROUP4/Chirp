using System.Globalization;
using System.Text;

namespace Chirp.Core.Utils;

public class StringUtils
{
    public static bool IsInteger(string x)
    {
        if (x.Length == 0)
        {
            return false;
        }

        if (x[0] == '-' && x.Length > 1)
        {
            x = x.Substring(1);
        }

        if (x[0] == '0' && x.Length > 1)
        {
            return false;
        }

        // Checks that the remaining characters in the string are digits
        return x.Select(x => char.IsDigit(x)).Aggregate((x, y) => x && y);
    }

    // Returns filename from file path
    // Given a file path such as: "/dir/foo/example.txt"
    // This will return "example.txt"
    //
    // KNOWN BUG:
    // Will not work correctly with windows root filepaths i.e. "c:example.txt" will return "c:example.txt"
    // Although will handle other cases fine: "c:\dir\example.txt" will return "example.txt"
    public static string GetFileName(string filepath)
    {
        int begin = -1;
        for (int i = filepath.Length - 1; i >= 0; i--)
        {
            if (filepath[i] == '/' || filepath[i] == '\\')
            {
                begin = i;
                break;
            }
        }

        if (begin == -1)
        {
            return filepath;
        }

        return filepath.Substring(begin + 1);
    }

    // Strips away filename from filepath
    // I.e. "/dir/foo/example.txt" will reutrn "/dir/foo"
    // File extension is not required. I.e. "/dir/foo" will return "/dir"
    public static string GetDirectoryPath(string filepath) 
    {
        int end = -1;
        for(int i = filepath.Length - 1; i >= 0; i--)
        {
            if(filepath[i] == '/' || filepath[i] == '\\')
            {
                end = i;
                break;
            }
        }
        if(end == -1) return filepath;
        return filepath.Substring(0, end + 1);
    }

    // Generates unique file path inside given directory with a specific file format extension
    public static string UniqueFilePath(string dir, string format) 
    {
        Random rng = new();
        string candidate = Path.Join(dir, "" + rng.NextInt64() + format);

        while(Path.Exists(candidate))
        {
            candidate = Path.Join(dir, "" + rng.NextInt64() + format);
        }

        return candidate;
    }

    public static string CollectionToString<T>(IEnumerable<T> collection)
    {
        StringBuilder sb = new();

        sb.Append("[");

        sb.Append(String.Join(", ", collection));

        sb.Append("]");

        return sb.ToString();
    }

    // Converts a DateTimeOffset to a string using date, month, year format instead of the default month, date, year
    public static string TimeToString(DateTimeOffset time) 
    {
        string format = "dd/MM/yy HH:mm:ss";
        CultureInfo locale = CultureInfo.CurrentCulture;

        return time.ToString(format, locale);
    }
}