using System.Linq;
using System.Text.RegularExpressions;

namespace Assets
{
    internal static class StringExtensions
    {
        public static string Raw(this string s)
        {
            return Regex.Replace(s, @"\W", "").ToUpperInvariant();
        }

        public static string[] Raw(this string[] sa)
        {
            return sa.Select(s => s.Raw()).ToArray();
        }
    }
}