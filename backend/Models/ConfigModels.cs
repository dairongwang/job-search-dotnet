using System.Text.Json.Serialization;

namespace JobSearchDashboard.Api.Models;

public class AppConfig
{
    public int Version { get; set; } = 1;
    public string? UpdatedAt { get; set; }
    public List<SiteConfig> Sites { get; set; } = new();
    public JobPreferences Preferences { get; set; } = new();
    public SearchSettings Search { get; set; } = new();
}

public class SiteConfig
{
    public string Id { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public int MaxResults { get; set; } = 20;
    public Dictionary<string, object> Options { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class JobPreferences
{
    public List<string> Titles { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public List<string> Industries { get; set; } = new();
    public List<string> Locations { get; set; } = new();
    public List<string> Workplaces { get; set; } = new(); // remote, hybrid, onsite
    public List<string> JobTypes { get; set; } = new();
    public SalaryRange Salary { get; set; } = new();
    public int? MaxAgeDays { get; set; }
}

public class SalaryRange
{
    public int? Minimum { get; set; }
    public int? Maximum { get; set; }
    public string Currency { get; set; } = "USD";
    public string Period { get; set; } = "year";
    public bool IncludeUnlisted { get; set; } = true;
}

public class SearchSettings
{
    public bool StrictFiltering { get; set; } = false;
    public bool DedupeByUrl { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 15;
    public string UserAgent { get; set; } = "JobSearchDashboard/1.0";
}

public class JobListing
{
    public string Title { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string? Location { get; set; }
    public string? Workplace { get; set; }
    public string? Url { get; set; }
    public string? Source { get; set; }
    public string? SiteId { get; set; }
    public string? Description { get; set; }
    public string? SalaryText { get; set; }
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? SalaryCurrency { get; set; }
    public string? SalaryPeriod { get; set; }
    public string? JobType { get; set; }
    public string? Schedule { get; set; }
    public string? Industry { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Published { get; set; }
    public string? ExternalId { get; set; }
    public double Score { get; set; }
}
