# Pruebas

## Stack de Pruebas
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: xUnit Assert + FluentAssertions (opcional)
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
│   │   ├── Entities/
│   │   │   └── ResourceTests.cs
│   │   ├── ValueObjects/
│   │   │   └── ResourceNameTests.cs
│   │   └── Specifications/
│   │       └── ValidResourceSpecificationTests.cs
│   └── Application/
│       └── Services/
│           └── CreateResourceUseCaseTests.cs
│
├── integration/
│   ├── Repositories/
│   │   └── ResourceRepositoryTests.cs
│   └── Api/
│       └── ResourceControllerTests.cs
│
└── acceptance/
    └── Features/
        └── ResourceFeatureTests.cs
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

## Ejemplo de Prueba Unitaria

```csharp
public class ResourceTests
{
    [Fact]
    public void Create_ShouldCreateResource_WhenNameIsValid()
    {
        // Arrange
        var name = new ResourceName("Test Resource");
        
        // Act
        var resource = Resource.Create(name);
        
        // Assert
        Assert.NotNull(resource);
        Assert.Equal("Test Resource", resource.Name.Value);
        Assert.NotEqual(Guid.Empty, resource.Id);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => 
            Resource.Create(new ResourceName(invalidName)));
    }
}
```

## Ejemplo de Prueba con Mock

```csharp
public class CreateResourceUseCaseTests
{
    private readonly Mock<IResourceRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateResourceUseCase _sut;
    
    public CreateResourceUseCaseTests()
    {
        _repositoryMock = new Mock<IResourceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreateResourceUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task Execute_ShouldCreateResource_WhenValidRequest()
    {
        // Arrange
        var request = new CreateResourceRequest { Name = "Test" };
        
        // Act
        var result = await _sut.ExecuteAsync(request);
        
        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Resource>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
```
