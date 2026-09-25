namespace poc_sdd_net_core_api.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Technologies { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public int? DurationMonths { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Project() { }

    public static Project Create(
        Guid clientId,
        string title,
        string description,
        string technologies,
        DateTime startDate,
        int? durationMonths = null)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId is required", nameof(clientId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        if (string.IsNullOrWhiteSpace(technologies))
            throw new ArgumentException("Technologies is required", nameof(technologies));

        if (durationMonths is <= 0)
            throw new ArgumentException("DurationMonths must be greater than 0", nameof(durationMonths));

        return new Project
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Title = title,
            Description = description,
            Technologies = technologies,
            StartDate = startDate,
            DurationMonths = durationMonths,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        Guid clientId,
        string title,
        string description,
        string technologies,
        DateTime startDate,
        int? durationMonths = null)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId is required", nameof(clientId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        if (string.IsNullOrWhiteSpace(technologies))
            throw new ArgumentException("Technologies is required", nameof(technologies));

        if (durationMonths is <= 0)
            throw new ArgumentException("DurationMonths must be greater than 0", nameof(durationMonths));

        ClientId = clientId;
        Title = title;
        Description = description;
        Technologies = technologies;
        StartDate = startDate;
        DurationMonths = durationMonths;
    }
}
