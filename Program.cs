using WowGuildAPI.WowMappers;
using Microsoft.AspNetCore.Mvc;
using WowGuildAPI.SwaggerFilters;
using WowGuildAPI.Services;
using MongoDB.Driver;
using WowGuildAPI.Respository.GuildRespository;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Respository.CharacterRespository.Interfaces;
using WowGuildAPI.Respository.CharacterRespository;
using WowGuildAPI.Services.Interfaces;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var mongoDb = Environment.GetEnvironmentVariable("MONGO_DB");
var redis = Environment.GetEnvironmentVariable("REDIS");

if (!string.IsNullOrEmpty(mongoDb))
    builder.Configuration["ConnectionStrings:MongoDB"] = mongoDb;

if (!string.IsNullOrEmpty(redis))
    builder.Configuration["ConnectionStrings:Redis"] = redis;

builder.Logging.AddConsole();

// Cache support
builder.Services.AddResponseCaching();
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    options.Configuration = redisConnectionString;
    options.InstanceName = "WowGuildCache:";
});

// Add services
builder.Services.AddScoped<IRaiderIoGuildProfileService, RaiderIoGuildProfileService>();
builder.Services.AddScoped<IRaiderIoCharacterProfileService, RaiderIoCharacterProfileService>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Add Repositories
builder.Services.AddScoped<IGuildRepository, GuildRepository>();
builder.Services.AddScoped<IRaidProgressionsRepository, RaidProgressionsRepository>();
builder.Services.AddScoped<IRaidRankingsRepository, RaidRankingsRespository>();
builder.Services.AddScoped<IMembersRepository, MembersRespository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IMythicPlusScoresRepository, MythicPlusScoresRepository>();
builder.Services.AddScoped<IMythicPlusBestRunsRepository, MythicPlusBestRunsRepository>();

builder.Services.AddHttpClient();

// Add Mappers
builder.Services.AddAutoMapper(typeof(GuildMapper));

builder.Services.AddControllers(option =>
    option.CacheProfiles.Add("Cache60",
        new CacheProfile()
        {
            Duration = 60
        }));

// Swagger config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
    option.OperationFilter<RegionSchemaOperationFilter>();
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Wow Guild API",
        Version = "v1",
        Description = "API to get information about Wow guilds and characters"
    });
});

// CORS policy
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins("").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("PoliticaCors");

app.MapControllers();

app.Run();
