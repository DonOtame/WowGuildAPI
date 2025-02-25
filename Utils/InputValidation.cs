using WowGuildAPI.Models.Enums;

namespace WowGuildAPI.Utils
{
    public static class InputValidation
    {
        private static readonly string[] ValidRegions = { Region.us, Region.eu, Region.tw, Region.kr, Region.cn };

        public const string NameRealmRequiredMessage = "Both name and realm are required.";
        public const string InvalidRegionMessage = "Invalid region.";
        public static string ValidateRequest(string region, string name, string realm)
        {
            if (!AreValidParameters(name, realm))
                return NameRealmRequiredMessage;

            if (!IsValidRegion(region))
                return InvalidRegionMessage;

            return null;
        }

        private static bool AreValidParameters(string name, string realm)
        {
            return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(realm);
        }

        private static bool IsValidRegion(string region)
        {
            return !string.IsNullOrWhiteSpace(region) && ValidRegions.Contains(region);
        }

    }
}
