using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Clients.Commands.UpdateClient;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Clients.Commands;

public class UpdateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly UpdateClientCommandHandler _sut;
    private readonly Faker _faker;

    public UpdateClientCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMappingProfile>()).CreateMapper();
        _sut = new UpdateClientCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _mapper);

        _faker = new Faker();
    }

    private UpdateClientCommand GenerateCommand() =>
        new(_faker.Random.Guid(), _faker.Name.FullName(), _faker.Internet.Email(), _faker.Phone.PhoneNumber(), _faker.Internet.Url());

    [Fact]
    public async Task Handle_ShouldUpdate_WhenExists()
    {
        // Arrange
        var command = GenerateCommand();
        var client = Client.Create("Old Name", "old@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command with { Id = client.Id }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.Logo.Should().Be(command.Logo);
        _repositoryMock.Verify(r => r.UpdateAsync(client), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldClearLogo_WhenLogoIsNull()
    {
        // Arrange
        var command = GenerateCommand() with { Logo = null };
        var client = Client.Create("Old Name", "old@example.com", null, "https://cdn.example.com/old.png");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command with { Id = client.Id }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Logo.Should().BeNull();
        client.Logo.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenLogoIsNotAUrl()
    {
        // Arrange
        var command = GenerateCommand() with { Logo = "javascript:alert(1)" };
        var client = Client.Create("Old Name", "old@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

        // Act
        var act = () => _sut.Handle(command with { Id = client.Id }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var command = GenerateCommand();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
