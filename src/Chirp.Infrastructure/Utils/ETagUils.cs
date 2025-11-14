namespace Chirp.Infrastructure.Utils;

public static class ETagUtils
{
    public static string ToBase64(byte[] bytes) => Convert.ToBase64String(bytes);
    public static byte[] FromBase64(string base64) => Convert.FromBase64String(base64);

    public static byte[] NewValue() => Guid.NewGuid().ToByteArray();
}
