using System.Globalization;

namespace Milky.Extensions
{
    internal static class DoubleExtensions
    {
        /// <summary>
        /// An extension to format a <see cref="double"/> with <see cref="CultureInfo.InvariantCulture"/>
        /// </summary>
        /// <param name="value">The number to format</param>
        /// <param name="format">Composite shit</param>
        /// <returns>A formatted string</returns>
        public static string FormatInvariantCulture(this double value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
