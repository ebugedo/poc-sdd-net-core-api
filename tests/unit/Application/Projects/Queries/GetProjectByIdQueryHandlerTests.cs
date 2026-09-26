using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Projects.Queries.GetProjectById;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetProjectByIdQueryHandler _sut;
    private readonly Faker _faker;

    public GetProjectByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProjectMappingProfile>()).CreateMapper();
        _sut = new GetProjectByIdQueryHandler(_repositoryMock.Object, _mapper);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldReturnProject_WhenExists()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var sectorId = _faker.Random.Guid();
        var project = Project.Create(
            clientId,
            sectorId,
            "Portal",
            "Intranet",
            ".NET 8, PostgreSQL",
            _faker.Date.Future(),
            6);

        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

        // Act
        var result = await _sut.Handle(new GetProjectByIdQuery(project.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.ClientId.Should().Be(clientId);
        result.SectorId.Should().Be(sectorId);
        result.Title.Should().Be("Portal");
        result.Description.Should().Be("Intranet");
        result.Technologies.Should().Be(".NET 8, PostgreSQL");
        result.DurationMonths.Should().Be(6);
    }

    [Fact]
    public async Task Handle_ShouldThrowProjectNotFoundException_WhenNotExists()
    {
        // Arrange
        var id = _faker.Random.Guid();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        // Act
        var act = () => _sut.Handle(new GetProjectByIdQuery(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectNotFoundException>();
    }
}
