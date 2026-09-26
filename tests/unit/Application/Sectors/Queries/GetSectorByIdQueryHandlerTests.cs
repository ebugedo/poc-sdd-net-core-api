using AutoMapper;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Sectors.Queries.GetSectorById;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Sectors.Queries.GetSectorById;

public class GetSectorByIdQueryHandlerTests
{
    private readonly Mock<ISectorRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetSectorByIdQueryHandler _sut;

    public GetSectorByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISectorRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<SectorMappingProfile>()).CreateMapper();
        _sut = new GetSectorByIdQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnSector_WhenExists()
    {
        // Arrange
        var sectorId = Sector.TransporteId;
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sectorId))
            .ReturnsAsync(Sector.Create(sectorId, "Transporte"));

        // Act
        var result = await _sut.Handle(new GetSectorByIdQuery(sectorId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sectorId);
        result.Name.Should().Be("Transporte");
    }

    [Fact]
    public async Task Handle_ShouldThrowSectorNotFoundException_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sector?)null);

        // Act
        var act = () => _sut.Handle(new GetSectorByIdQuery(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SectorNotFoundException>();
    }
}
