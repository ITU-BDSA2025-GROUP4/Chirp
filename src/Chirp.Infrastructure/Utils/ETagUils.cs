namespace Chirp.Infrastructure.Utils;

public static class ETagUtils
{
    // Encodes a byte-array ETag as a base64 string
    public static string ToBase64(byte[] bytes) => Convert.ToBase64String(bytes);

    // Decodes a base64 string back into an ETag byte array
    public static byte[] FromBase64(string base64) => Convert.FromBase64String(base64);

    // Generates a new ETag value
    public static byte[] NewValue() => Guid.NewGuid().ToByteArray();
}