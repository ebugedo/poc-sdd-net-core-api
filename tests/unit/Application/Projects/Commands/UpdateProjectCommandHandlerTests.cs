using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Projects.Commands.UpdateProject;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Projects.Commands;

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ISectorRepository> _sectorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly UpdateProjectCommandHandler _sut;
    private readonly Faker _faker;

    public UpdateProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _sectorRepositoryMock = new Mock<ISectorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProjectMappingProfile>()).CreateMapper();
        _sut = new UpdateProjectCommandHandler(
            _repositoryMock.Object,
            _clientRepositoryMock.Object,
            _sectorRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapper);

        _faker = new Faker();
    }

    private UpdateProjectCommand GenerateCommand(Guid? clientId = null) =>
        new(
            _faker.Random.Guid(),
            clientId ?? _faker.Random.Guid(),
            _faker.Random.Guid(),
            _faker.Company.CompanyName(),
            _faker.Lorem.Sentence(),
            _faker.Lorem.Word(),
            _faker.Date.Future(),
            _faker.Random.Int(1, 24));

    private static Project CreateProject(Guid clientId) =>
        Project.Create(clientId, Guid.NewGuid(), "Old", "Old description", "Old stack", DateTime.UtcNow, 3);

    private void SetupExistingSector(Guid sectorId)
    {
        _sectorRepositoryMock
            .Setup(r => r.GetByIdAsync(sectorId))
            .ReturnsAsync(Sector.Create(sectorId, "Ingeniería"));
    }

    [Fact]
    public async Task Handle_ShouldUpdate_WhenExists()
    {
        // Arrange
        var command = GenerateCommand();
        var project = CreateProject(command.ClientId);
        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);
        _clientRepositoryMock
            .Setup(r => r.GetByIdAsync(command.ClientId))
            .ReturnsAsync(Client.Create("Acme", "acme@example.com"));
        SetupExistingSector(command.SectorId);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command with { Id = project.Id }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(command.Title);
        result.SectorId.Should().Be(command.SectorId);
        result.Description.Should().Be(command.Description);
        result.Technologies.Should().Be(command.Technologies);
        result.DurationMonths.Should().Be(command.DurationMonths);
        _repositoryMock.Verify(r => r.UpdateAsync(project), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        var command = GenerateCommand();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowClientNotFoundException_WhenNewClientDoesNotExist()
    {
        // Arrange
        var command = GenerateCommand();
        var project = CreateProject(command.ClientId);
        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var act = () => _sut.Handle(command with { Id = project.Id }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientNotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowSectorNotFoundException_WhenSectorDoesNotExist()
    {
        // Arrange
        var command = GenerateCommand();
        var project = CreateProject(command.ClientId);
        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);
        _clientRepositoryMock
            .Setup(r => r.GetByIdAsync(command.ClientId))
            .ReturnsAsync(Client.Create("Acme", "acme@example.com"));
        _sectorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sector?)null);

        // Act
        var act = () => _sut.Handle(command with { Id = project.Id }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SectorNotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenTechnologiesIsEmpty()
    {
        // Arrange
        var command = GenerateCommand() with { Technologies = "  " };
        var project = CreateProject(command.ClientId);
        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);
        _clientRepositoryMock
            .Setup(r => r.GetByIdAsync(command.ClientId))
            .ReturnsAsync(Client.Create("Acme", "acme@example.com"));
        SetupExistingSector(command.SectorId);

        // Act
        var act = () => _sut.Handle(command with { Id = project.Id }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
