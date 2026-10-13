using System.Text.Json;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Services;

public interface IConfigService
{
    Task<AppConfig> LoadAsync();
    Task SaveAsync(AppConfig config);
}

public class ConfigService : IConfigService
{
    private readonly string _path;
    private readonly ILogger<ConfigService> _logger;
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public ConfigService(IWebHostEnvironment env, ILogger<ConfigService> logger)
    {
        _logger = logger;
        var dir = Path.Combine(env.ContentRootPath, "config");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "job_search_config.json");
    }

    public async Task<AppConfig> LoadAsync()
    {
        if (!File.Exists(_path))
        {
            var defaults = CreateDefaults();
            await SaveAsync(defaults);
            return defaults;
        }
        try
        {
            var json = await File.ReadAllTextAsync(_path);
            return JsonSerializer.Deserialize<AppConfig>(json, Opts) ?? CreateDefaults();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load config, using defaults");
            return CreateDefaults();
        }
    }

    public async Task SaveAsync(AppConfig config)
    {
        config.UpdatedAt = DateTime.UtcNow.ToString("o");
        var json = JsonSerializer.Serialize(config, Opts);
        await File.WriteAllTextAsync(_path, json);
    }

    private static AppConfig CreateDefaults() => new()
    {
        Sites = new List<SiteConfig>
        {
            new() { Id = "remotive", Provider = "remotive", Name = "Remotive", Enabled = true, MaxResults = 20, Options = new() { ["category"] = "software-dev" } },
            new() { Id = "jobicy", Provider = "jobicy", Name = "Jobicy", Enabled = true, MaxResults = 20, Options = new() { ["geo"] = "usa" } },
            new() { Id = "arbeitnow", Provider = "arbeitnow", Name = "Arbeitnow", Enabled = false, MaxResults = 30 },
            new() { Id = "remoteok", Provider = "remoteok", Name = "Remote OK", Enabled = false, MaxResults = 30 }
        },
        Preferences = new JobPreferences
        {
            Titles = new() { "Software Engineer", "Data Engineer" },
            Skills = new() { "C#", "Python", "SQL", "Azure" },
            Workplaces = new() { "remote", "hybrid" }
        }
    };
}
