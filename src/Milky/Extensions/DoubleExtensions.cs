using System.Globalization;

namespace Milky.Extensions
{
    internal static class DoubleExtensions
    {
        public static string FormatInvariantCulture(this double value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
