using System.Globalization;
using Essentials.Numerics;

namespace Tests;

internal static class NumberTestExtensions
{
    public static double ToDouble(this Number value) =>
        double.Parse(value.ToString("R", CultureInfo.InvariantCulture),
            CultureInfo.InvariantCulture);
}
