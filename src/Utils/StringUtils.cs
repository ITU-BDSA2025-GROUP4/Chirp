using System.Globalization;

namespace Utils;

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

        return x.Select(x => char.IsDigit(x)).Aggregate((x, y) => x && y);
    }

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

    public static string TimeToString(DateTimeOffset time)
    {
        string format = "dd/MM/yy HH:mm:ss";
        CultureInfo locale = CultureInfo.CurrentCulture;

        return time.ToString(format, locale);
    }
}