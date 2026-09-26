using AutoMapper;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Sectors.Queries.GetAllSectors;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Sectors.Queries.GetAllSectors;

public class GetAllSectorsQueryHandlerTests
{
    private readonly Mock<ISectorRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllSectorsQueryHandler _sut;

    public GetAllSectorsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISectorRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<SectorMappingProfile>()).CreateMapper();
        _sut = new GetAllSectorsQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnTheSevenSectors()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Sector.DefaultCatalog);

        // Act
        var result = await _sut.Handle(new GetAllSectorsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(7);
        result.Select(s => s.Name).Should().Contain(new[]
        {
            "Administración pública",
            "Ingeniería",
            "Publicidad",
            "Servicios financieros",
            "Servicios tecnológicos",
            "Transporte",
            "Sector inmobiliario"
        });
        result.Should().OnlyContain(s => s.Id != Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenCatalogIsEmpty()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<Sector>());

        // Act
        var result = await _sut.Handle(new GetAllSectorsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
