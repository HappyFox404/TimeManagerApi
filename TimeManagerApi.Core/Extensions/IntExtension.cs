namespace TimeManagerApi.Core.Extensions;

public static class IntExtension
{
    public static DateTime ConvertTimestampToDateTime(this int value)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(value).ToLocalTime();
    }
}