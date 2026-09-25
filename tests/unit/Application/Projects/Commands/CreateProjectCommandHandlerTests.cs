using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Application.Projects.Commands.CreateProject;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateProjectCommandHandler _sut;
    private readonly Faker _faker;

    public CreateProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProjectMappingProfile>()).CreateMapper();
        _sut = new CreateProjectCommandHandler(
            _repositoryMock.Object,
            _clientRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapper);

        _faker = new Faker();
    }

    private CreateProjectCommand GenerateCommand(Guid? clientId = null) =>
        new(
            clientId ?? _faker.Random.Guid(),
            _faker.Company.CompanyName(),
            _faker.Lorem.Sentence(),
            _faker.Lorem.Word(),
            _faker.Date.Future(),
            _faker.Random.Int(1, 24));

    private void SetupExistingClient(Guid clientId)
    {
        _clientRepositoryMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(Client.Create("Acme", "acme@example.com"));
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnProject()
    {
        // Arrange
        var command = GenerateCommand();
        SetupExistingClient(command.ClientId);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync((Project p) => p);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.ClientId.Should().Be(command.ClientId);
        result.Description.Should().Be(command.Description);
        result.Technologies.Should().Be(command.Technologies);
        result.DurationMonths.Should().Be(command.DurationMonths);
        result.Id.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Project>(p => p.Title == command.Title)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateWithoutDuration_WhenDurationIsNull()
    {
        // Arrange
        var command = GenerateCommand() with { DurationMonths = null };
        SetupExistingClient(command.ClientId);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync((Project p) => p);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DurationMonths.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowClientNotFoundException_WhenClientDoesNotExist()
    {
        // Arrange
        var command = GenerateCommand();
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientNotFoundException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var command = GenerateCommand() with { Title = " " };
        SetupExistingClient(command.ClientId);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenDurationIsNotPositive()
    {
        // Arrange
        var command = GenerateCommand() with { DurationMonths = 0 };
        SetupExistingClient(command.ClientId);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Never);
    }
}
