namespace poc_sdd_net_core_api.Application.DTOs;

public class ProjectResponse
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int? DurationMonths { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProjectRequest
{
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int? DurationMonths { get; set; }
}

public class UpdateProjectRequest
{
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int? DurationMonths { get; set; }
}
