using System.Runtime.InteropServices;

namespace API.Contratual.CrossCutting.Common;

public static class DateTimeExtensions
{
    public static DateTime ToDateTimeBrazil(this DateTime dateTime)
    {
        var brasil = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ?
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo") :
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var dateTimeBrazil = TimeZoneInfo.ConvertTimeFromUtc(dateTime, brasil);
        return new DateTime(
            year: dateTimeBrazil.Year,
            month: dateTimeBrazil.Month,
            day: dateTimeBrazil.Day,
            hour: dateTimeBrazil.Hour,
            minute: dateTimeBrazil.Minute,
            second: dateTimeBrazil.Second,
            millisecond: dateTimeBrazil.Millisecond,
            kind: DateTimeKind.Local);
    }
}

// Chamada do metodo
//var dateCall = DateTime.UtcNow.ToDateTimeBrazil();