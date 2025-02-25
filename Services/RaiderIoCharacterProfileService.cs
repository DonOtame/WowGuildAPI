using System.Text.Json;
using WowGuildAPI.Models.CharacterModels;
using WowGuildAPI.Services.Interfaces;

namespace WowGuildAPI.Services
{
    public class RaiderIoCharacterProfileService : IRaiderIoCharacterProfileService
    {
        private readonly HttpClient _httpClient;

        public RaiderIoCharacterProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<JsonElement?> GetApiResponseAsync(string region, string realm, string name, string fields = "")
        {
            var apiUrl = $"https://raider.io/api/v1/characters/profile?region={region}&realm={realm}&name={name}{(string.IsNullOrEmpty(fields) ? "" : "&fields=" + fields)}";
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

        public async Task<Character?> GetCharacterAsync(string region, string realm, string name)
        {
            var characterData = await GetApiResponseAsync(region, realm, name);
            if (characterData == null) return null;

            var characterProperties = characterData.Value;

            return new Character
            {
                Name = ExtractProperty<string>(characterProperties, "name"),
                Race = ExtractProperty<string>(characterProperties, "race"),
                Class = ExtractProperty<string>(characterProperties, "class"),
                ActiveSpecName = ExtractProperty<string>(characterProperties, "active_spec_name"),
                ActiveSpecRole = ExtractProperty<string>(characterProperties, "active_spec_role"),
                Faction = ExtractProperty<string>(characterProperties, "faction"),
                ThumbnailUrl = ExtractProperty<string>(characterProperties, "thumbnail_url"),
                Region = ExtractProperty<string>(characterProperties, "region"),
                Realm = ExtractProperty<string>(characterProperties, "realm"),
                LastCrawledAt = ExtractProperty<DateTime>(characterProperties, "last_crawled_at"),
                ProfileUrl = ExtractProperty<string>(characterProperties, "profile_url"),
                LastUpdated = DateTime.Now
            };
        }
        public async Task<List<MythicPlusScores>> GetMythicPlusScoresAsync(string region, string realm, string name)
        {
            var currentRequest = GetApiResponseAsync(region, realm, name, "mythic_plus_scores_by_season:current");
            var previousRequest = GetApiResponseAsync(region, realm, name, "mythic_plus_scores_by_season:previous");

            await Task.WhenAll(currentRequest, previousRequest);

            var currentData = await currentRequest;
            var previousData = await previousRequest;

            return new List<MythicPlusScores>()
                .Concat(currentData?.TryGetProperty("mythic_plus_scores_by_season", out var currentSeasons) == true
                    ? currentSeasons.EnumerateArray().Select(season => MapMythicPlusScores(season, true))
                    : Enumerable.Empty<MythicPlusScores>())
                .Concat(previousData?.TryGetProperty("mythic_plus_scores_by_season", out var previousSeasons) == true
                    ? previousSeasons.EnumerateArray().Select(season => MapMythicPlusScores(season, false))
                    : Enumerable.Empty<MythicPlusScores>())
                .ToList();
        }


        private MythicPlusScores MapMythicPlusScores(JsonElement season, bool isCurrentSeason)
        {
            var validSegmentNames = new HashSet<string> { "all", "dps", "healer", "tank" };

            return new MythicPlusScores
            {
                Season = season.GetProperty("season").GetString() ?? string.Empty,
                IsCurrentSeason = isCurrentSeason,
                Segments = season.GetProperty("segments")
                    .EnumerateObject()
                    .Where(s => validSegmentNames.Contains(s.Name))
                    .ToDictionary(
                        s => s.Name,
                        s => new Segments
                        {
                            Score = s.Value.GetProperty("score").GetDouble(),
                            Color = s.Value.GetProperty("color").GetString() ?? "#ffffff"
                        })
            };
        }

        public async Task<List<MythicPlusBestRuns>> GetMythicPlusBestRunsAsync(string region, string realm, string name)
        {
            var response = await GetApiResponseAsync(region, realm, name, "mythic_plus_best_runs");
            if (response == null || !response.Value.TryGetProperty("mythic_plus_best_runs", out var bestRunsData))
                return new List<MythicPlusBestRuns>();

            return bestRunsData.EnumerateArray()
                .Select(run => new MythicPlusBestRuns
                {
                    Dungeon = run.GetProperty("dungeon").GetString() ?? string.Empty,
                    ShortName = run.GetProperty("short_name").GetString() ?? string.Empty,
                    MythicLevel = run.GetProperty("mythic_level").GetInt32(),
                    CompletedAt = run.GetProperty("completed_at").GetDateTime(),
                    ClearTimeMs = run.GetProperty("clear_time_ms").GetDouble(),
                    KeystoneRunId = run.GetProperty("keystone_run_id").GetInt32(),
                    ParTimeMs = run.GetProperty("par_time_ms").GetDouble(),
                    NumKeystoneUpgrades = run.GetProperty("num_keystone_upgrades").GetInt32(),
                    MapChallengeModeId = run.GetProperty("map_challenge_mode_id").GetInt32(),
                    ZoneId = run.GetProperty("zone_id").GetInt32(),
                    ZoneExpansionId = run.GetProperty("zone_expansion_id").GetInt32(),
                    IconUrl = run.GetProperty("icon_url").GetString() ?? string.Empty,
                    BackgroundImageUrl = run.GetProperty("background_image_url").GetString() ?? string.Empty,
                    Score = run.GetProperty("score").GetDouble(),
                    Url = run.GetProperty("url").GetString() ?? string.Empty,
                    Affixes = run.GetProperty("affixes")
                        .EnumerateArray()
                        .Select(Affix => MapAffix(Affix))
                        .ToList()
                })
                .ToList();
        }

        private Affix MapAffix(JsonElement affix) => new Affix
        {
            Id = affix.GetProperty("id").GetInt32(),
            Name = affix.GetProperty("name").GetString() ?? string.Empty,
            Description = affix.GetProperty("description").GetString() ?? string.Empty,
            IconUrl = affix.GetProperty("icon_url").GetString() ?? string.Empty
        };

    }
}
