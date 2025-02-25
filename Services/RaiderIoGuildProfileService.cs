using System.Text.Json;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Services.Interfaces;

namespace WowGuildAPI.Services
{
    public class RaiderIoGuildProfileService : IRaiderIoGuildProfileService
    {
        private readonly HttpClient _httpClient;

        public RaiderIoGuildProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        private async Task<JsonElement?> GetApiResponseAsync(string region, string realm, string name, string fields = "")
        {
            var apiUrl = $"https://raider.io/api/v1/guilds/profile?region={region}&realm={realm}&name={name}{(string.IsNullOrEmpty(fields) ? "" : "&fields=" + fields)}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var guildData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            return guildData;
        }

        private T ExtractProperty<T>(JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var property))
            {
                return property.Deserialize<T>();
            }
            return default;
        }

        public async Task<Guild?> GetGuildAsync(string region, string realm, string name)
        {
            var guildData = await GetApiResponseAsync(region, realm, name);
            if (guildData == null) return null;

            var guildProperties = guildData.Value;

            return new Guild
            {
                Region = ExtractProperty<string>(guildProperties, "region"),
                Realm = ExtractProperty<string>(guildProperties, "realm"),
                Name = ExtractProperty<string>(guildProperties, "name"),
                Faction = ExtractProperty<string>(guildProperties, "faction"),
                LastCrawledAt = ExtractProperty<DateTime>(guildProperties, "last_crawled_at"),
                ProfileUrl = ExtractProperty<string>(guildProperties, "profile_url"),
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<Dictionary<string, RaidProgression>> GetRaidProgressionsAsync(string region, string realm, string name)
        {
            var guildData = await GetApiResponseAsync(region, realm, name, "raid_progression");
            if (guildData == null || !guildData.Value.TryGetProperty("raid_progression", out var raidProgressionData))
                return new Dictionary<string, RaidProgression>();

            return raidProgressionData.EnumerateObject()
                .ToDictionary(raid => raid.Name, raid =>
                {
                    var raidSlug = raid.Name;
                    var raidEncounters = GetRaidEncountersAsync(region, realm, name, raidSlug).Result;

                    return new RaidProgression
                    {
                        Summary = ExtractProperty<string>(raid.Value, "summary"),
                        BackgroundImageUrl = $"https://cdn.raiderio.net/images/{raid.Name}/headers/_zone.jpg",
                        TotalBosses = ExtractProperty<int>(raid.Value, "total_bosses"),
                        BossesKilled = new BossKilled
                        {
                            Normal = ExtractProperty<int>(raid.Value, "normal_bosses_killed"),
                            Heroic = ExtractProperty<int>(raid.Value, "heroic_bosses_killed"),
                            Mythic = ExtractProperty<int>(raid.Value, "mythic_bosses_killed")
                        },
                        RaidEncounters = raidEncounters
                    };
                });
        }


        private async Task<Dictionary<string, RaidEncounter>> GetRaidEncountersAsync(string region, string realm, string name, string raidSlug)
        {
            var encounters = new Dictionary<string, RaidEncounter>();
            var difficulties = new[] { "normal", "heroic", "mythic" };

            foreach (var difficulty in difficulties)
            {
                var guildData = await GetApiResponseAsync(region, realm, name, $"raid_encounters:{raidSlug}:{difficulty}");
                if (guildData == null || !guildData.Value.TryGetProperty("raid_encounters", out var encountersData))
                    continue;

                foreach (var encounter in encountersData.EnumerateArray())
                {
                    var bossSlug = ExtractProperty<string>(encounter, "slug");
                    var bossName = ExtractProperty<string>(encounter, "name");
                    var defeatDate = ExtractProperty<DateTime>(encounter, "defeatedAt");
                    if (!encounters.TryGetValue(bossSlug, out var raidEncounter))
                    {
                        raidEncounter = new RaidEncounter
                        {
                            Slug = raidSlug,
                            Name = bossName,
                            BossImageUrl = $"https://cdn.raiderio.net/cdn-cgi/image/quality=75,width=205/images/{raidSlug}/portraits/{bossSlug}.png",
                            Defeats = new Dictionary<string, DateTime>()
                        };
                        encounters[bossSlug] = raidEncounter;
                    }
                    raidEncounter.Defeats[difficulty] = defeatDate;
                }
            }
            return encounters;
        }



        public async Task<Dictionary<string, RaidRanking>> GetRaidRankingsAsync(string region, string realm, string name)
        {
            var guildData = await GetApiResponseAsync(region, realm, name, "raid_rankings");
            if (guildData == null || !guildData.Value.TryGetProperty("raid_rankings", out var raidRankingData))
                return new Dictionary<string, RaidRanking>();

            var raidRankings = raidRankingData.EnumerateObject()
                .ToDictionary(raid => raid.Name, raid => new RaidRanking
                {
                    Normal = MapRaidLevel(raid.Value.GetProperty("normal")),
                    Heroic = MapRaidLevel(raid.Value.GetProperty("heroic")),
                    Mythic = MapRaidLevel(raid.Value.GetProperty("mythic"))
                });

            return raidRankings;
        }

        private RaidLevel MapRaidLevel(JsonElement raidLevelData) => new RaidLevel
        {
            World = raidLevelData.GetProperty("world").GetInt32(),
            Region = raidLevelData.GetProperty("region").GetInt32(),
            Realm = raidLevelData.GetProperty("realm").GetInt32()
        };

        public async Task<List<Member>> GetMembersAsync(string region, string realm, string name)
        {
            var guildData = await GetApiResponseAsync(region, realm, name, "members");
            if (guildData == null || !guildData.Value.TryGetProperty("members", out var membersData))
                return new List<Member>();

            return membersData.EnumerateArray()
                .Select(m => new Member
                {
                    Rank = m.GetProperty("rank").GetInt32(),
                    Character = MapCharacter(m.GetProperty("character"))
                })
                .Where(m => m.Rank is 0 or 1 or 2)
                .ToList();
        }

        private Character MapCharacter(JsonElement charData) => new Character
        {
            Name = charData.GetProperty("name").GetString() ?? string.Empty,
            Region = charData.GetProperty("region").GetString() ?? string.Empty,
            Realm = charData.GetProperty("realm").GetString() ?? string.Empty
        };



    }
}
