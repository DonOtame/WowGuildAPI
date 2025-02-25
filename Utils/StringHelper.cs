using System.Text.RegularExpressions;

namespace WowGuildAPI.Utils
{
    public static class StringHelper
    {
        public static string CleanInput(string input)
        {
            return Regex.Replace(input.ToLower(), @"[^a-z0-9\s]", "");
        }
    }
}
