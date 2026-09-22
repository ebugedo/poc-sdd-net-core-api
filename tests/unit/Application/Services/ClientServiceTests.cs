using Bogus;
using FluentAssertions;
using Moq;
using Xunit;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Application.Services;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace UnitTests.Application.Services;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClientService _sut;
    private readonly Faker<CreateClientRequest> _createFaker;
    private readonly Faker<UpdateClientRequest> _updateFaker;

    public ClientServiceTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ClientService(_repositoryMock.Object, _unitOfWorkMock.Object);

        _createFaker = new Faker<CreateClientRequest>()
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber());

        _updateFaker = new Faker<UpdateClientRequest>()
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllClients()
    {
        // Arrange
        var clients = new List<Client>
        {
            Client.Create("John Doe", "john@example.com", "+123"),
            Client.Create("Jane Doe", "jane@example.com")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(clients);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnClient_WhenExists()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

        // Act
        var result = await _sut.GetByIdAsync(client.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(client.Id);
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnClient()
    {
        // Arrange
        var request = _createFaker.Generate();
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync((Client c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Email.Should().Be(request.Email);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Client>(c => c.Name == request.Name)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenExists()
    {
        // Arrange
        var client = Client.Create("Old Name", "old@example.com");
        var request = _updateFaker.Generate();
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateAsync(client.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.Email.Should().Be(request.Email);
        _repositoryMock.Verify(r => r.UpdateAsync(client), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var request = _updateFaker.Generate();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteAsync(client.Id);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(client), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
}
