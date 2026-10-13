using System.Net.Http.Headers;
using System.Text.Json;
using JobSearchDashboard.Api.Models;

namespace JobSearchDashboard.Api.Services;

public interface IJobSearchService
{
    Task<List<JobListing>> SearchAsync(AppConfig config, string? keywordOverride = null, CancellationToken ct = default);
    Task<List<JobListing>> TestSiteAsync(SiteConfig site, AppConfig config, CancellationToken ct = default);
}

public class JobSearchService : IJobSearchService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<JobSearchService> _logger;

    public JobSearchService(IHttpClientFactory httpFactory, ILogger<JobSearchService> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task<List<JobListing>> SearchAsync(AppConfig config, string? keywordOverride = null, CancellationToken ct = default)
    {
        var keywords = keywordOverride != null
            ? new[] { keywordOverride }
            : config.Preferences.Titles.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

        if (keywords.Length == 0) keywords = new[] { "" };

        var enabled = config.Sites.Where(s => s.Enabled).ToList();
        var all = new List<JobListing>();

        foreach (var site in enabled)
        {
            try
            {
                var jobs = await FetchSiteAsync(site, config, keywords, ct);
                all.AddRange(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch from site {Site}", site.Id);
            }
        }

        // Dedupe by URL
        if (config.Search.DedupeByUrl)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            all = all.Where(j =>
            {
                var key = (j.Url ?? j.ExternalId ?? $"{j.Title}|{j.Organization}").Trim().ToLowerInvariant();
                if (seen.Contains(key)) return false;
                seen.Add(key);
                return true;
            }).ToList();
        }

        // Simple ranking
        foreach (var job in all)
        {
            job.Score = ScoreJob(job, config.Preferences);
        }

        return all.OrderByDescending(j => j.Score).Take(200).ToList();
    }

    public Task<List<JobListing>> TestSiteAsync(SiteConfig site, AppConfig config, CancellationToken ct = default)
    {
        var keywords = config.Preferences.Titles.Take(1).DefaultIfEmpty("software").ToArray();
        return FetchSiteAsync(site, config, keywords, ct);
    }

    private async Task<List<JobListing>> FetchSiteAsync(SiteConfig site, AppConfig config, string[] keywords, CancellationToken ct)
    {
        var client = _httpFactory.CreateClient("jobsearch");
        client.Timeout = TimeSpan.FromSeconds(config.Search.TimeoutSeconds);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(config.Search.UserAgent);

        return site.Provider.ToLowerInvariant() switch
        {
            "remotive" => await FetchRemotiveAsync(client, site, keywords, ct),
            "jobicy" => await FetchJobicyAsync(client, site, keywords, ct),
            "arbeitnow" => await FetchArbeitnowAsync(client, site, keywords, ct),
            "remoteok" => await FetchRemoteOkAsync(client, site, keywords, ct),
            _ => new List<JobListing>()
        };
    }

    private async Task<List<JobListing>> FetchRemotiveAsync(HttpClient client, SiteConfig site, string[] keywords, CancellationToken ct)
    {
        var results = new List<JobListing>();
        var category = site.Options.TryGetValue("category", out var c) ? c?.ToString() : null;
        var limit = site.MaxResults;

        foreach (var kw in keywords.Take(2))
        {
            var url = "https://remotive.com/api/remote-jobs";
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(kw)) qs.Add($"search={Uri.EscapeDataString(kw)}");
            if (!string.IsNullOrWhiteSpace(category)) qs.Add($"category={Uri.EscapeDataString(category)}");
            if (qs.Count > 0) url += "?" + string.Join("&", qs);

            try
            {
                var json = await client.GetStringAsync(url, ct);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("jobs", out var jobs))
                {
                    foreach (var j in jobs.EnumerateArray().Take(limit))
                    {
                        results.Add(MapRemotive(j, site));
                        if (results.Count >= limit) break;
                    }
                }
            }
            catch { /* continue */ }
            if (results.Count >= limit) break;
        }
        return results.Take(limit).ToList();
    }

    private JobListing MapRemotive(JsonElement j, SiteConfig site) => new()
    {
        Title = j.GetProperty("title").GetString() ?? "",
        Organization = j.TryGetProperty("company_name", out var cn) ? cn.GetString() : null,
        Location = j.TryGetProperty("candidate_required_location", out var loc) ? loc.GetString() : "Remote",
        Workplace = "remote",
        Url = j.TryGetProperty("url", out var u) ? u.GetString() : null,
        Source = site.Name,
        SiteId = site.Id,
        Description = j.TryGetProperty("description", out var d) ? d.GetString() : null,
        SalaryText = j.TryGetProperty("salary", out var s) ? s.GetString() : null,
        Tags = j.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Array
            ? tags.EnumerateArray().Select(t => t.GetString() ?? "").Where(t => t.Length > 0).ToList()
            : new(),
        Published = j.TryGetProperty("publication_date", out var pd) ? pd.GetString()?.Split('T')[0] : null,
        ExternalId = j.TryGetProperty("id", out var id) ? id.ToString() : null
    };

    private async Task<List<JobListing>> FetchJobicyAsync(HttpClient client, SiteConfig site, string[] keywords, CancellationToken ct)
    {
        var results = new List<JobListing>();
        var geo = site.Options.TryGetValue("geo", out var g) ? g?.ToString() : null;
        var limit = site.MaxResults;

        foreach (var kw in keywords.Take(2))
        {
            var url = "https://jobicy.com/api/v2/remote-jobs";
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(kw)) qs.Add($"tag={Uri.EscapeDataString(kw)}");
            if (!string.IsNullOrWhiteSpace(geo)) qs.Add($"geo={Uri.EscapeDataString(geo)}");
            if (qs.Count > 0) url += "?" + string.Join("&", qs);

            try
            {
                var json = await client.GetStringAsync(url, ct);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("jobs", out var jobs))
                {
                    foreach (var j in jobs.EnumerateArray().Take(limit))
                    {
                        results.Add(MapJobicy(j, site));
                        if (results.Count >= limit) break;
                    }
                }
            }
            catch { }
            if (results.Count >= limit) break;
        }
        return results.Take(limit).ToList();
    }

    private JobListing MapJobicy(JsonElement j, SiteConfig site) => new()
    {
        Title = j.TryGetProperty("jobTitle", out var t) ? t.GetString() ?? "" : "",
        Organization = j.TryGetProperty("companyName", out var c) ? c.GetString() : null,
        Location = j.TryGetProperty("jobGeo", out var loc) ? loc.GetString() : "Remote",
        Workplace = "remote",
        Url = j.TryGetProperty("url", out var u) ? u.GetString() : null,
        Source = site.Name,
        SiteId = site.Id,
        Description = j.TryGetProperty("jobExcerpt", out var d) ? d.GetString() : null,
        SalaryText = j.TryGetProperty("annualSalaryMin", out var smin) && j.TryGetProperty("annualSalaryMax", out var smax)
            ? $"{smin}-{smax}" : null,
        Published = j.TryGetProperty("pubDate", out var pd) ? pd.GetString()?.Split('T')[0] : null
    };

    private async Task<List<JobListing>> FetchArbeitnowAsync(HttpClient client, SiteConfig site, string[] keywords, CancellationToken ct)
    {
        var results = new List<JobListing>();
        var limit = site.MaxResults;
        var url = "https://www.arbeitnow.com/api/job-board-api";

        try
        {
            var json = await client.GetStringAsync(url, ct);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("data", out var data))
            {
                foreach (var j in data.EnumerateArray().Take(limit * 2))
                {
                    var title = j.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                    if (keywords.Length > 0 && keywords.Any(k => !string.IsNullOrEmpty(k) &&
                        !title.Contains(k, StringComparison.OrdinalIgnoreCase))) continue;

                    results.Add(new JobListing
                    {
                        Title = title,
                        Organization = j.TryGetProperty("company_name", out var cn) ? cn.GetString() : null,
                        Location = j.TryGetProperty("location", out var loc) ? loc.GetString() : null,
                        Workplace = j.TryGetProperty("remote", out var rem) && rem.GetBoolean() ? "remote" : "onsite",
                        Url = j.TryGetProperty("url", out var u) ? u.GetString() : null,
                        Source = site.Name,
                        SiteId = site.Id,
                        Description = j.TryGetProperty("description", out var d) ? d.GetString() : null,
                        Tags = j.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Array
                            ? tags.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                            : new(),
                        Published = j.TryGetProperty("created_at", out var ca) ? ca.GetString()?.Split('T')[0] : null
                    });
                    if (results.Count >= limit) break;
                }
            }
        }
        catch { }
        return results;
    }

    private async Task<List<JobListing>> FetchRemoteOkAsync(HttpClient client, SiteConfig site, string[] keywords, CancellationToken ct)
    {
        var results = new List<JobListing>();
        var limit = site.MaxResults;
        var url = "https://remoteok.com/api";

        try
        {
            var json = await client.GetStringAsync(url, ct);
            using var doc = JsonDocument.Parse(json);
            // First element is metadata
            var arr = doc.RootElement.EnumerateArray().Skip(1);
            foreach (var j in arr.Take(limit * 2))
            {
                var title = j.TryGetProperty("position", out var p) ? p.GetString() ?? "" : "";
                if (keywords.Length > 0 && keywords.Any(k => !string.IsNullOrEmpty(k) &&
                    !title.Contains(k, StringComparison.OrdinalIgnoreCase))) continue;

                results.Add(new JobListing
                {
                    Title = title,
                    Organization = j.TryGetProperty("company", out var c) ? c.GetString() : null,
                    Location = j.TryGetProperty("location", out var loc) ? loc.GetString() : "Remote",
                    Workplace = "remote",
                    Url = j.TryGetProperty("url", out var u) ? u.GetString() : null,
                    Source = site.Name,
                    SiteId = site.Id,
                    Description = j.TryGetProperty("description", out var d) ? d.GetString() : null,
                    SalaryText = j.TryGetProperty("salary_min", out var smin) && j.TryGetProperty("salary_max", out var smax)
                        ? $"{smin}-{smax}" : null,
                    Tags = j.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Array
                        ? tags.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                        : new()
                });
                if (results.Count >= limit) break;
            }
        }
        catch { }
        return results;
    }

    private static double ScoreJob(JobListing job, JobPreferences prefs)
    {
        double score = 1.0;
        var text = $"{job.Title} {job.Organization} {job.Description} {string.Join(" ", job.Tags)}".ToLowerInvariant();

        foreach (var skill in prefs.Skills)
            if (text.Contains(skill.ToLowerInvariant())) score += 2;

        foreach (var title in prefs.Titles)
            if (job.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) score += 3;

        if (prefs.Workplaces.Count > 0 && !string.IsNullOrEmpty(job.Workplace) &&
            prefs.Workplaces.Contains(job.Workplace, StringComparer.OrdinalIgnoreCase))
            score += 1;

        return score;
    }
}
