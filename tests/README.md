# Pruebas

## Stack de Pruebas
- **Framework**: xUnit
- **Mocking**: Moq
- **Fake Data**: Bogus
- **Assertions**: FluentAssertions
- **Integration Tests**: WebApplicationFactory + Testcontainers (PostgreSQL)

## Tipos de Pruebas

### `/unit`
Pruebas unitarias que verifican el comportamiento aislado de componentes.
- **Domain**: Entidades, Value Objects, Specifications
- **Application**: Use Cases, Services
- **NO Infrastructure**: Se mockea

### `/integration`
Pruebas de integración que verifican la interacción entre componentes.
- **Repository + PostgreSQL**: Usando Testcontainers
- **API + Database**: Flujo completo

### `/acceptance`
Pruebas de aceptación que verifican los requisitos del negocio.
- **Escenarios de usuario**: Dado/Cuando/Entonces
- **API E2E**: Requests HTTP completos

## Estrategia de Pruebas

### Cobertura Mínima
| Tipo | Cobertura Objetivo |
|------|-------------------|
| Unit Tests | 80% |
| Integration Tests | 60% |
| Acceptance Tests | Criterios principales |

### Qué testear
```
✅ Domain: Entidades, Value Objects, Specifications, Domain Services
✅ Application: Use Cases, Validación de DTOs
✅ Infrastructure: Repositories (con integración)
❌ API Controllers: Solo integración
```

## Estructura de Pruebas

```
tests/
├── unit/
│   ├── Domain/
│   │   └── Entities/
│   │       └── ClientTests.cs              # xUnit + FluentAssertions
│   └── Application/
│       └── Services/
│           └── ClientServiceTests.cs       # xUnit + Moq + Bogus + FluentAssertions
│
├── integration/
│   ├── Repositories/
│   │   └── ClientRepositoryTests.cs        # futuro Testcontainers
│   └── Api/
│       └── ClientsControllerTests.cs       # futuro WebApplicationFactory
│
└── acceptance/
    └── Features/
        └── ClientFeatureTests.cs           # futuro
```

## Ejecución

```bash
# Todas las pruebas
dotnet test

# Solo unit tests
dotnet test --filter "Category=Unit"

# Solo integration tests
dotnet test --filter "Category=Integration"

# Con cobertura
dotnet test /p:CollectCoverage=true

# Reporte de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Ejemplo de Prueba Unitaria con Bogus y FluentAssertions

```csharp
public class ResourceTests
{
    private readonly Faker<Resource> _resourceFaker = new Faker<Resource>()
        .RuleFor(r => r.Id, f => Guid.NewGuid())
        .RuleFor(r => r.Name, f => f.Lorem.Word());

    [Fact]
    public void Create_ShouldCreateResource_WhenNameIsValid()
    {
        // Arrange
        var name = new ResourceName("Test Resource");
        
        // Act
        var resource = Resource.Create(name);
        
        // Assert
        resource.Should().NotBeNull();
        resource.Name.Value.Should().Be("Test Resource");
        resource.Id.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange & Act & Assert
        var act = () => Resource.Create(new ResourceName(invalidName));
        act.Should().Throw<DomainException>();
    }
    
    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var resources = Enumerable.Range(0, 10)
            .Select(_ => Resource.Create(new ResourceName("Test")))
            .ToList();
        
        // Assert
        resources.Select(r => r.Id).Should().OnlyHaveUniqueItems();
    }
}
```

## Ejemplo de Prueba con Mock y Bogus (Moq)

```csharp
public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClientService _sut;
    private readonly Faker<CreateClientRequest> _requestFaker;
    
    public ClientServiceTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ClientService(_repositoryMock.Object, _unitOfWorkMock.Object);
        _requestFaker = new Faker<CreateClientRequest>()
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.Email, f => f.Internet.Email());
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnClient()
    {
        // Arrange
        var request = _requestFaker.Generate();
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync((Client c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Client>(c => c.Name == request.Name)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```
