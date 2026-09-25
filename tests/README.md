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
- **Application**: Command handlers y Query handlers (CQRS)
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
✅ Application: Command handlers, Query handlers, validación de DTOs
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
│       └── Clients/
│           ├── Commands/
│           │   ├── CreateClientCommandHandlerTests.cs   # xUnit + Moq + Bogus + FluentAssertions
│           │   ├── UpdateClientCommandHandlerTests.cs
│           │   └── DeleteClientCommandHandlerTests.cs
│           └── Queries/
│               ├── GetAllClientsQueryHandlerTests.cs
│               └── GetClientByIdQueryHandlerTests.cs
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
public class ClientTests
{
    [Fact]
    public void Create_ShouldCreateClient_WhenValidData()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";
        
        // Act
        var client = Client.Create(name, email);
        
        // Assert
        client.Should().NotBeNull();
        client.Name.Should().Be(name);
        client.Email.Should().Be(email);
        client.Id.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange & Act & Assert
        var act = () => Client.Create(invalidName!, "john@example.com");
        act.Should().Throw<ArgumentException>();
    }
}
```

## Ejemplo de Prueba con Mock y Bogus (Moq)

Los handlers se testean directamente (sin mediator): se mockean `IClientRepository` e `IUnitOfWork`, y el mapper real se construye con `MapperConfiguration`.

```csharp
public class CreateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateClientCommandHandler _sut;
    private readonly Faker _faker;

    public CreateClientCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMappingProfile>()).CreateMapper();
        _sut = new CreateClientCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _mapper);
        _faker = new Faker();
    }

    private CreateClientCommand GenerateCommand() =>
        new(_faker.Name.FullName(), _faker.Internet.Email(), _faker.Phone.PhoneNumber());

    [Fact]
    public async Task Handle_ShouldCreateAndReturnClient()
    {
        // Arrange
        var command = GenerateCommand();
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync((Client c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Client>(c => c.Name == command.Name)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

> **Nota**: los commands/queries son `record` posicionales, sin constructor sin parámetros. Bogus no puede instanciarlos: usa `new Faker()` y un factory (`GenerateCommand()`) en lugar de `Faker<TCommand>`.

Estado actual: **21 pruebas** en verde (`dotnet test`).

