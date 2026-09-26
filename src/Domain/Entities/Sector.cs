namespace poc_sdd_net_core_api.Domain.Entities;

public class Sector
{
    public static readonly Guid AdministracionPublicaId = new("11111111-1111-1111-1111-111111111111");
    public static readonly Guid IngenieriaId = new("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PublicidadId = new("33333333-3333-3333-3333-333333333333");
    public static readonly Guid ServiciosFinancierosId = new("44444444-4444-4444-4444-444444444444");
    public static readonly Guid ServiciosTecnologicosId = new("55555555-5555-5555-5555-555555555555");
    public static readonly Guid TransporteId = new("66666666-6666-6666-6666-666666666666");
    public static readonly Guid SectorInmobiliarioId = new("77777777-7777-7777-7777-777777777777");

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Sector() { }

    public static Sector Create(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id is required", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        return new Sector
        {
            Id = id,
            Name = name
        };
    }

    /// <summary>
    /// Catálogo cerrado de sectores (BR-016). Es de solo lectura: los ids son fijos
    /// para que la siembra por migración sea idempotente y estable entre entornos.
    /// </summary>
    public static IReadOnlyList<Sector> DefaultCatalog => new[]
    {
        Create(AdministracionPublicaId, "Administración pública"),
        Create(IngenieriaId, "Ingeniería"),
        Create(PublicidadId, "Publicidad"),
        Create(ServiciosFinancierosId, "Servicios financieros"),
        Create(ServiciosTecnologicosId, "Servicios tecnológicos"),
        Create(TransporteId, "Transporte"),
        Create(SectorInmobiliarioId, "Sector inmobiliario")
    };
}
