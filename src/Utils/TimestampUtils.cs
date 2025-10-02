namespace Utils;

using System.Globalization;

public static class TimestampUtils {
    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static string DateTimeTimeStampToDateTimeString(DateTime timestamp)
    {
        return timestamp.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static DateTime DateTimeStringToDateTimeTimeStamp(string timestamp)
    {
        CultureInfo ct = new("da-DK");
        return DateTime.Parse(timestamp, ct);
    }
}