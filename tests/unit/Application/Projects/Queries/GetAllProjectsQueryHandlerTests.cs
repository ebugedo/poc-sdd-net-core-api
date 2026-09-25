using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Projects.Queries.GetAllProjects;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllProjectsQueryHandler _sut;
    private readonly Faker _faker;

    public GetAllProjectsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProjectMappingProfile>()).CreateMapper();
        _sut = new GetAllProjectsQueryHandler(_repositoryMock.Object, _mapper);
        _faker = new Faker();
    }

    private Project GenerateProject(Guid clientId) =>
        Project.Create(
            clientId,
            _faker.Company.CompanyName(),
            _faker.Lorem.Sentence(),
            _faker.Lorem.Word(),
            _faker.Date.Future(),
            _faker.Random.Int(1, 24));

    [Fact]
    public async Task Handle_ShouldReturnAllProjects()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var projects = new List<Project> { GenerateProject(clientId), GenerateProject(clientId) };
        _repositoryMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(projects);

        // Act
        var result = await _sut.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Title).Should().BeEquivalentTo(projects.Select(p => p.Title));
        result.Should().OnlyContain(p => p.Id != Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldFilterByClientId()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var project = GenerateProject(clientId);
        _repositoryMock.Setup(r => r.GetAllAsync(clientId)).ReturnsAsync(new List<Project> { project });

        // Act
        var result = await _sut.Handle(new GetAllProjectsQuery(clientId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(project.Id);
        result[0].ClientId.Should().Be(clientId);
        _repositoryMock.Verify(r => r.GetAllAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProjects()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Guid?>())).ReturnsAsync(Array.Empty<Project>());

        // Act
        var result = await _sut.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapNullDuration()
    {
        // Arrange
        var project = Project.Create(
            _faker.Random.Guid(),
            "Portal",
            "Intranet",
            ".NET 8",
            _faker.Date.Future());

        _repositoryMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(new List<Project> { project });

        // Act
        var result = await _sut.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].DurationMonths.Should().BeNull();
    }
}
