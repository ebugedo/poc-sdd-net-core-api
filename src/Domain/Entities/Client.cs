namespace poc_sdd_net_core_api.Domain.Entities;

public class Client
{
    private const int LogoMaxLength = 2048;

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Logo { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Client() { }

    public static Client Create(string name, string email, string? phone = null, string? logo = null)
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
            Logo = NormalizeLogo(logo),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email, string? phone = null, string? logo = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Name = name;
        Email = email;
        Phone = phone;
        Logo = NormalizeLogo(logo);
    }

    private static string? NormalizeLogo(string? logo)
    {
        if (string.IsNullOrWhiteSpace(logo))
            return null;

        var value = logo.Trim();

        if (value.Length > LogoMaxLength)
            throw new ArgumentException($"Logo must not exceed {LogoMaxLength} characters", nameof(logo));

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Logo must be a valid absolute URL", nameof(logo));
        }

        return value;
    }
}
