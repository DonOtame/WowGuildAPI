using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;

namespace WowGuildAPI.Utils
{
    public static class WowUtils
    {
        public static ObjectId GenerateWowId(string region, string realm, string name)
        {
            var cleanRegion = StringHelper.CleanInput(region);
            var cleanRealm = StringHelper.CleanInput(realm);
            var cleanName = StringHelper.CleanInput(name);

            var combined = $"{cleanRegion}-{cleanRealm}-{cleanName}";

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(combined);
                var hashBytes = sha256.ComputeHash(bytes);

                var truncatedHash = new byte[12];
                Array.Copy(hashBytes, truncatedHash, 12);

                return new ObjectId(truncatedHash);
            }
        }

        public static bool IsUpdateRequired(DateTime lastUpdated)
        {
            return (DateTime.UtcNow - lastUpdated).TotalHours > 1;
        }

    }
}
