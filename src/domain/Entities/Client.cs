namespace poc_sdd_net_core_api.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Client() { }

    public static Client Create(string name, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        return new Client
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Name = name;
        Email = email;
        Phone = phone;
    }
}
