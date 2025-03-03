namespace Common.SupportedFormat;

public enum Format
{
    JPG,
    PNG
}

public static class SupportedFormats
{
    private static readonly Dictionary<Format, string> _formatMessages = new()
    {
        { Format.JPG, ".jpg" },
        { Format.PNG, ".png" }
    };

    public static string GetFormat(Format formatCode)
    {
        return _formatMessages.GetValueOrDefault(formatCode, "Format undefined");
    }


    public static string GetAllFormats()
    {
        return string.Join(",", _formatMessages.Values);
    }

    public static bool isPermitedFormat(string format)
    {
        return _formatMessages.ContainsValue(format);
    }
}