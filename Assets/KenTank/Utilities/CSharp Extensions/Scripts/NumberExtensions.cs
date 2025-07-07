namespace KenTank.Utilities.Extensions
{
    public static class NumberExtensions
    {
        public static string ToShortNumberString(this double target, string decimalFormat = "0.##")
        {
            if (target >= 1_000_000_000)
                return (target / 1_000_000_000D).ToString(decimalFormat) + "B"; // Miliar
            if (target >= 1_000_000)
                return (target / 1_000_000D).ToString(decimalFormat) + "M"; // Juta
            if (target >= 1_000)
                return (target / 1_000D).ToString(decimalFormat) + "K"; // Ribu

            return target.ToString();
        }

        public static string ToShortNumberString(this float target, string decimalFormat = "0.##")
        {
            return ((double)target).ToShortNumberString(decimalFormat);
        }
        public static string ToShortNumberString(this int target, string decimalFormat = "0.##")
        {
            return ((double)target).ToShortNumberString(decimalFormat);
        }
    }
}