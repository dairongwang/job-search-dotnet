namespace JobSearchDashboard.Api.Models;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string? Location { get; set; }
    public string? Workplace { get; set; } // remote | hybrid | onsite
    public string? Url { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = "Saved"; // Saved, Applied, Interview, Offer, Rejected, Closed
    public string Priority { get; set; } = "Medium";
    public string? Salary { get; set; }
    public string? DateFound { get; set; }
    public string? DateApplied { get; set; }
    public string? NextAction { get; set; }
    public string? NextActionDate { get; set; }
    public string? Notes { get; set; }
    public string? Description { get; set; }
    public string? ExternalId { get; set; }
    public string? JobType { get; set; }
    public string? Schedule { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string? Title { get; set; }
    public string? Email { get; set; }
    public string? LinkedIn { get; set; }
    public string? Phone { get; set; }
    public string? Relationship { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Outreach
{
    public int Id { get; set; }
    public string? ContactName { get; set; }
    public string? Organization { get; set; }
    public string? JobTitle { get; set; }
    public string Channel { get; set; } = "Email"; // Email, LinkedIn, Call, etc.
    public string? Subject { get; set; }
    public string ActivityDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public string? FollowUpDate { get; set; }
    public string? Outcome { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ResearchNote
{
    public int Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
